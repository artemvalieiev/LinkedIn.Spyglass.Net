using Android.Content;

namespace LinkedIn.Spyglass.Sample;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.samples);

        FindViewById(Resource.Id.simple_sample)!
            .Click += (sender, args) => StartActivity(new Intent(this, typeof(SimpleMentions)));

        FindViewById(Resource.Id.colorful_sample)!
            .Click += (sender, args) => StartActivity(new Intent(this, typeof(ColorfulMentions)));

        FindViewById(Resource.Id.networked_sample)!
            .Click += (sender, args) => StartActivity(new Intent(this, typeof(NetworkedMentions)));

        FindViewById(Resource.Id.multi_source_sample)!
            .Click += (sender, args) => StartActivity(new Intent(this, typeof(MultiSourceMentions)));

        FindViewById(Resource.Id.grid_sample)!
             .Click += (sender, args) => StartActivity(new Intent(this, typeof(GridMentions)));
    }
}
