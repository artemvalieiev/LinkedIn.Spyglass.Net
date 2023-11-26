using AndroidX.AppCompat.App;
using LinkedIn.Spyglass.Suggestions;
using LinkedIn.Spyglass.Tokenization;
using LinkedIn.Spyglass.Ui;

namespace LinkedIn.Spyglass.Sample;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.SpyglassSample", MainLauncher = false)]
public class ColorfulMentions : AppCompatActivity, IQueryTokenReceiver
{
    private const string Bucket = "cities-memory";

    // replace this with your editor view
    private RichEditorView editor; // You will need to install a rich text editor NuGet package or build a custom one
    private City.CityLoader cities;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.colorful_mentions);
        editor = FindViewById<RichEditorView>(Resource.Id.editor); // replace RichEditorView with your custom view.
        editor.SetQueryTokenReceiver(this);
        editor.SetHint(Resources.GetString(Resource.String.type_city));
        cities = new City.CityLoader(Resources);
    }

    public IList<string> OnQueryReceived(QueryToken queryToken)
    {
        var buckets = new List<string>() { Bucket };
        var suggestions = cities.GetSuggestions(queryToken.TokenString);
        var result = new SuggestionsResult(queryToken, suggestions.Cast<ISuggestible>().ToList());
        editor.OnReceiveSuggestionsResult(result, Bucket);
        return buckets;
    }
}