using Android.Content;
using Android.Content.Res;
using Android.Views;
using AndroidX.AppCompat.App;
using LinkedIn.Spyglass.Suggestions;
using LinkedIn.Spyglass.Tokenization;
using LinkedIn.Spyglass.Ui;

namespace LinkedIn.Spyglass.Sample;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.SpyglassSample", MainLauncher = false)]
public class MultiSourceMentions : AppCompatActivity, IQueryTokenReceiver
{
    private const string PersonBucket = "people-database";
    private const string CityBucket = "city-network";
    private const int PersonDelay = 10;
    private const int CityDelay = 2000;


    private RichEditorView editor; // Needs a proper definition OR a Nuget Package.
    private CheckBox peopleCheckBox;
    private CheckBox citiesCheckBox;

    private City.CityLoader cities; // Proper class definition required
    private Person.PersonLoader people; // Proper class definition required

    private SuggestionsResult lastPersonSuggestions; // Proper class definition required
    private SuggestionsResult lastCitySuggestions; // Proper class definition required

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.multi_source_mentions);

        editor = FindViewById<RichEditorView>(Resource.Id.editor);
        editor.SetQueryTokenReceiver(this);
        editor.SetSuggestionsListBuilder(new CustomSuggestionsListBuilder());

        people = new Person.PersonLoader(Resources);
        cities = new City.CityLoader(Resources);

        peopleCheckBox = FindViewById<CheckBox>(Resource.Id.person_checkbox);
        peopleCheckBox.CheckedChange += (_, _) => UpdateSuggestions();

        citiesCheckBox = FindViewById<CheckBox>(Resource.Id.city_checkbox);
        citiesCheckBox.CheckedChange += (_, _) => UpdateSuggestions();

        UpdateSuggestions();
    }

    private void UpdateSuggestions()
    {
        var hasPeople = peopleCheckBox.Checked;
        var hasCities = citiesCheckBox.Checked;

// Handle person mentions
        if (hasPeople && lastPersonSuggestions != null)
        {
            editor.OnReceiveSuggestionsResult(lastPersonSuggestions, PersonBucket);
        }
        else if (lastPersonSuggestions != null)
        {
            var emptySuggestions = new SuggestionsResult(lastPersonSuggestions.QueryToken, new List<ISuggestible>());
            editor.OnReceiveSuggestionsResult(emptySuggestions, PersonBucket);
        }

// Handle city mentions
        if (hasCities && lastCitySuggestions != null)
        {
            editor.OnReceiveSuggestionsResult(lastCitySuggestions, CityBucket);
        }
        else if (lastCitySuggestions != null)
        {
            var emptySuggestions = new SuggestionsResult(lastCitySuggestions.QueryToken, new List<ISuggestible>());
            editor.OnReceiveSuggestionsResult(emptySuggestions, CityBucket);
        }

        switch (hasPeople, hasCities)
        {
            // Update the hint
            case (true, true):
                editor.SetHint(GetString(Resource.String.type_both));
                break;
            case (true, false):
                editor.SetHint(GetString(Resource.String.type_person));
                break;
            case (false, true):
                editor.SetHint(GetString(Resource.String.type_city));
                break;
            default:
                editor.SetHint(GetString(Resource.String.type_neither));
                break;
        }
    }

    public IList<string> OnQueryReceived(QueryToken queryToken)
    {
        // This seems like Java's equivalent to casting,
        // but the actual interface may not be IQueryTokenReceiver
        var hasPeople = peopleCheckBox.Checked;
        var hasCities = citiesCheckBox.Checked;

        var buckets = new List<string>();

        if (hasPeople)
        {
            buckets.Add(PersonBucket);
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(PersonDelay));
                var suggestions = people.GetSuggestions(queryToken.TokenString);
                // QueryToken and SuggestionsResult classes need to be implemented
                lastPersonSuggestions = new SuggestionsResult(queryToken, suggestions.Cast<ISuggestible>().ToList());
                editor.OnReceiveSuggestionsResult(lastPersonSuggestions, PersonBucket);

                // Please replace `RunOnUiThread` with the correct method to run code on UI thread
                RunOnUiThread(() => editor.OnReceiveSuggestionsResult(lastPersonSuggestions, PersonBucket));
            });
        }

        if (hasCities)
        {
            buckets.Add(CityBucket);
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(CityDelay));
                var suggestions = cities.GetSuggestions(queryToken.TokenString);
                lastCitySuggestions = new SuggestionsResult(queryToken, suggestions.Cast<ISuggestible>().ToList());

                // Please replace `RunOnUiThread` with the correct method to run code on UI thread
                RunOnUiThread(() => editor.OnReceiveSuggestionsResult(lastCitySuggestions, CityBucket));
            });
        }

        return buckets;
    }

    /*
    The inner class CustomSuggestionsListBuilder is considered a bit talky to convert to C# due to
    Requires a proper overriden method of Android BaseAdapter class
    */


    private class CustomSuggestionsListBuilder : BasicSuggestionsListBuilder
    {
        public override View GetView(ISuggestible suggestion, View? convertView, ViewGroup? parent,
            Context context,
            LayoutInflater inflater, Resources resources)
        {
            var v = base.GetView(suggestion, convertView, parent, context, inflater, resources);
            if (!(v is not TextView)) return v;

            // Color text depending on the type of mention
            var tv = (TextView)v;
            if (suggestion is Person)
                tv.SetTextColor(resources.GetColor(Resource.Color.person_mention_text));
            else if (suggestion is City) tv.SetTextColor(resources.GetColor(Resource.Color.city_mention_text));

            return tv;
        }
    }
}