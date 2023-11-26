using Android.OS;
using LinkedIn.Spyglass.Mentions;
using Android.Content.Res;
using LinkedIn.Spyglass.Suggestions;
using Newtonsoft.Json;

namespace LinkedIn.Spyglass.Sample;

public class City : Java.Lang.Object, IMentionable, ISuggestible
{
    public string Name { get; set; }

    public City(string name)
    {
        Name = name;
    }

    private City(Parcel parcel)
    {
        Name = parcel.ReadString();
    }

    public int DescribeContents()
    {
        return 0;
    }

    public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
    {
        dest.WriteString(Name);
    }

    public IMentionable.MentionDeleteStyle DeleteStyle => IMentionable.MentionDeleteStyle.PartialNameDelete;

    public string GetTextForDisplayMode(IMentionable.MentionDisplayMode mode)
    {
        return mode == IMentionable.MentionDisplayMode.Full ? Name : "";
    }


    public string GetSuggestiblePrimaryText()
    {
        return Name;
    }


    public class CityLoader : MentionsLoader<City>
    {
        private const string Tag = nameof(CityLoader);

        public CityLoader(Resources res) : base(res, Resource.Raw.us_cities)
        {
        }

        protected override City[]? DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject<string[]>(json).Select(x => new City(x)).ToArray();
        }
    }

    public int SuggestibleId => Name.GetHashCode();

    public string SuggestiblePrimaryText => Name;
}

public class Person : Java.Lang.Object, IMentionable
{
    private string mFirstName;
    private string mLastName;
    private string mPictureURL;

    public Person()
    {
        
    }


    [JsonProperty("first")]
    public string FirstName
    {
        get => mFirstName;
        set => mFirstName = value;
    }

    [JsonProperty("last")]
    public string LastName
    {
        get => mLastName;
        set => mLastName = value;
    }

    [JsonProperty("picture")]
    public string PictureUrl
    {
        get=> mPictureURL;
        set => mPictureURL = value;
    }

    public string FullName => FirstName + " " + LastName;

    public string getPictureURL()
    {
        return mPictureURL;
    }

    public int DescribeContents()
    {
        return 0;
    }

    public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
    {
        dest.WriteString(mFirstName);
        dest.WriteString(mLastName);
        dest.WriteString(mPictureURL);
    }

    public int SuggestibleId => FullName.GetHashCode();
    public string SuggestiblePrimaryText => FullName;
    public IMentionable.MentionDeleteStyle DeleteStyle => IMentionable.MentionDeleteStyle.PartialNameDelete;

    public string GetTextForDisplayMode(IMentionable.MentionDisplayMode mode)
    {
        if (mode == IMentionable.MentionDisplayMode.Full)
            return FullName;
        else if (mode == IMentionable.MentionDisplayMode.Partial)
            return FullName.Split(" ").FirstOrDefault() ?? string.Empty;
        else
            return "";
    }

    public class PersonLoader : MentionsLoader<Person>
    {
        public PersonLoader(Resources res) : base(res, Resource.Raw.people)
        {
        }

        protected override Person[]? DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject<Person[]>(json);
        }
    }
}

public abstract class MentionsLoader<T> where T : IMentionable
{
    protected T[]? Data;
    private const string Tag = nameof(MentionsLoader<T>);

    protected MentionsLoader(Resources res, int resId)
    {
        _ = ReadDataAsync(res, resId);
    }

    protected async Task ReadDataAsync(Resources res, int resId)
    {
        await using var fileReader = res.OpenRawResource(resId);
        using var reader = new StreamReader(fileReader);
        var json = await reader.ReadToEndAsync();

        Data = DeserializeObject(json);
    }
    
    protected virtual T[]? DeserializeObject(string json) => JsonConvert.DeserializeObject<T[]>(json);

    public List<T> GetSuggestions(string queryToken)
    {
        var prefix = queryToken.ToLower();
        List<T> suggestions = null;
        if (Data != null)
            suggestions = Data.Where(Data => Data.SuggestiblePrimaryText.ToLower().StartsWith(prefix)).ToList();

        return suggestions ?? new List<T>();
    }
}