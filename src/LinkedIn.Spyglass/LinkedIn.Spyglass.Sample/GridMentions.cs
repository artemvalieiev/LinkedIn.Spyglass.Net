using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using LinkedIn.Spyglass.Suggestions;
using LinkedIn.Spyglass.Tokenization;
using LinkedIn.Spyglass.Ui;

namespace LinkedIn.Spyglass.Sample;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.SpyglassSample", MainLauncher = false, WindowSoftInputMode = SoftInput.AdjustResize)]
public class GridMentions : AppCompatActivity, IQueryTokenReceiver, ISuggestionsResultListener,
    ISuggestionsVisibilityManager
{
    private const string Bucket = "people-network";

    private RecyclerView _recyclerView;
    private MentionsEditText _editor;
    private PersonMentionAdapter _adapter;
    private Person.PersonLoader _people;
    private bool _isDisplayingSuggestions;

    private WordTokenizerConfig tokenizerConfig = new WordTokenizerConfig
            .Builder()
        .SetWordBreakChars(", ")
        .SetExplicitChars("@")
        .SetMaxNumKeywords(2)
        .SetThreshold(2)
        .Build();

    private WordTokenizer wordTokenizer;
    private CardView _recyclerViewCardView;


    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        SetContentView(Resource.Layout.grid_mentions);

        _recyclerView = FindViewById<RecyclerView>(Resource.Id.mentions_grid)!;
        _recyclerViewCardView = _recyclerView.Parent as CardView;
        _recyclerView.SetLayoutManager(new LinearLayoutManager(this, RecyclerView.Vertical, false));


        _editor = FindViewById<MentionsEditText>(Resource.Id.editor)!;
        // The tokenizer setup requires additional implementation of WordTokenizer
        wordTokenizer = new WordTokenizer(tokenizerConfig);
        _editor.Tokenizer = wordTokenizer;
        _editor.SetQueryTokenReceiver(this);
        _editor.SetSuggestionsVisibilityManager(this);
        _editor.SetHint(Resource.String.type_person_new);

        _adapter = new PersonMentionAdapter(new List<Person>(), _editor, _recyclerView, ItemClick);
        _recyclerView.SetAdapter(_adapter);

        _people = new Person.PersonLoader(Resources);
    }

    // WordTokenizer.IQueryTokenReceiver Implementation
    public IList<string> OnQueryReceived(QueryToken queryToken)
    {
   
        List<string> buckets = new List<string>() { Bucket };
        if(!queryToken.IsExplicit)
        {
            return buckets;
        }
        
        List<Person> suggestions = _people.GetSuggestions(queryToken.TokenString.Replace("@", ""));
        SuggestionsResult result = new SuggestionsResult(queryToken, suggestions.Cast<ISuggestible>().ToList());
        // Have suggestions, now call the listener (which is this activity)
        OnReceiveSuggestionsResult(result, Bucket);
        return buckets;
    }

    // WordTokenizer.ISuggestionsResultListener Implementation
    void OnReceiveSuggestionsResult(SuggestionsResult result, string bucket)
    {
        var suggestions = result.Suggestions;
        _adapter = new PersonMentionAdapter(suggestions.OfType<Person>().ToList(), _editor, _recyclerView, ItemClick);
        _recyclerView.SwapAdapter(_adapter, true);
        bool display = suggestions is { Count: > 0 };
        DisplaySuggestions(display);
    }

    private void ItemClick(Person person)
    {
        _editor.InsertMention(person);
        _recyclerView.SwapAdapter(new PersonMentionAdapter(new List<Person>(), _editor, _recyclerView, ItemClick),
            true);
        DisplaySuggestions(false);
        _editor.RequestFocus();
    }
    
    bool ISuggestionsVisibilityManager.IsDisplayingSuggestions => _recyclerView.Visibility == ViewStates.Visible;

    void ISuggestionsVisibilityManager.DisplaySuggestions(bool p0)
    {
        DisplaySuggestions(p0);
    }

    void DisplaySuggestions(bool display)
    {
        _recyclerViewCardView.Visibility = display ? ViewStates.Visible : ViewStates.Gone;
        _recyclerView.Visibility = display ? ViewStates.Visible : ViewStates.Gone;
    }

    void ISuggestionsResultListener.OnReceiveSuggestionsResult(SuggestionsResult p0, string p1)
    {
        OnReceiveSuggestionsResult(p0, p1);
    }
}

public class PersonMentionAdapter : RecyclerView.Adapter
{
    private readonly Action<Person> _itemClick;
    private List<Person> suggestions;
    private readonly MentionsEditText _editor;
    private readonly RecyclerView _recyclerView;

    public PersonMentionAdapter(List<Person> people, MentionsEditText editor, RecyclerView recyclerView,
        Action<Person> itemClick)
    {
        _itemClick = itemClick;
        suggestions = people;
        _editor = editor;
        _recyclerView = recyclerView;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        View v = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.grid_mention_item, parent, false)!;

        var myViewHolder = new MyViewHolder(v);
        myViewHolder.ItemView.Click += (s, e) => ItemViewOnClick(myViewHolder.Person);
        return myViewHolder;
    }

    private void ItemViewOnClick(Person person)
    {
        _itemClick?.Invoke(person);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
    {
        var suggestion = suggestions[position];
        if (suggestion is not Person person) return;
        if (viewHolder is not MyViewHolder holder) return;
        holder.SetPerson(person);
    }

    public override int ItemCount => suggestions.Count;
}

public class MyViewHolder : RecyclerView.ViewHolder
{
    public TextView name;
    public ImageView picture;
    private Person person;

    public Person Person => person;

    public MyViewHolder(View itemView) : base(itemView)
    {
        name = (TextView)itemView.FindViewById(Resource.Id.person_name);
        //picture = (ImageView) itemView.FindViewById(Resource.Id.person_image);
    }

    public void SetPerson(Person person)
    {
        name.Text = person.FullName;
        this.person = person;
    }
}