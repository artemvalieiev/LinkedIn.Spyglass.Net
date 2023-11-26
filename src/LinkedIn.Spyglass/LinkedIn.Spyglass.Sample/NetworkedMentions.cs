using Android.OS;
using AndroidX.AppCompat.App;
using Java.Util;
using LinkedIn.Spyglass.Mentions;
using LinkedIn.Spyglass.Suggestions;
using LinkedIn.Spyglass.Tokenization;
using LinkedIn.Spyglass.Ui;

namespace LinkedIn.Spyglass.Sample;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.SpyglassSample", MainLauncher = false)]
public class NetworkedMentions : AppCompatActivity, IQueryTokenReceiver
{
    private const string Bucket = "people-network";

    private TextView _delayLabel;
    private SeekBar _delaySeek;
    private RichEditorView _editor;
    private City.CityLoader _cities;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        // Set layout
        SetContentView(Resource.Layout.networked_mentions);

        // Find Views
        _delayLabel = FindViewById<TextView>(Resource.Id.label_network_delay);
        _delaySeek = FindViewById<SeekBar>(Resource.Id.seek_network_delay);
        _editor = FindViewById<RichEditorView>(Resource.Id.editor);

        // Update network delay text
        _delaySeek.ProgressChanged += (s, e) => UpdateDelayLabel(e.FromUser, e.Progress);

        // Initialize CityLoader
        _cities = new City.CityLoader(Resources);
    }

    private void UpdateDelayLabel(bool fromUser, int progress)
    {
        if (fromUser)
        {
            var progressValue = (float)progress / 10;
            _delayLabel.Text = "Mock Network Delay: " + progressValue.ToString("0.0") + " seconds";
        }
    }

    private int GetDelayValue()
    {
        return _delaySeek.Progress * 100;
    }

    public IList<string> OnQueryReceived(QueryToken queryToken)
    {
        var buckets = new List<string>();

        var listener = _editor;
        var handler = new Handler(Looper.MainLooper);
        handler.PostDelayed(() =>
        {
            var suggestions = _cities.GetSuggestions(queryToken.TokenString);
            var result = new SuggestionsResult(queryToken, suggestions.Cast<ISuggestible>().ToList());
            listener.OnReceiveSuggestionsResult(result, "cities-memory");
        }, GetDelayValue());

        return buckets;
    }
}