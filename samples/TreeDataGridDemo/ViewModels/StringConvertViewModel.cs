using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Bogus;
using Bogus.DataSets;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace TreeDataGridDemo.ViewModels
{
    public class StringConvertViewModel : ReactiveObject {
    private string? _filterText;

    public StringConvertViewModel() {
        var data = new ObservableCollection<Person>(GenerateFakes(3000)).ToObservableChangeSet(person => person.Id);

        var searchFilter = this.WhenValueChanged(t => t.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Select(BuildSearchFilter);

        var filteredData = data.Filter(searchFilter);

        filteredData.Bind(out var items);
        
        

        DataSource =
            new FlatTreeDataGridSource<Person>(items) {
                Columns = {
                    new TextColumn<Person, int>("Id", person => person.Id),
                    new TextColumn<Person, string>("FirstName", person => person.FirstName),
                    new TextColumn<Person, string>("LastName", person => person.LastName),
                    new TextColumn<Person, DateTime>("DoB", person => person.DateOfBirth),
                    new TemplateColumn<Person>("Height", "HeightCell",
                        options: new TemplateColumnOptions<Person> {
                            CompareAscending = (a, b) => {
                                if (a?.Height > b?.Height)
                                    return 1;

                                if (a?.Height == b?.Height)
                                    return 0;

                                return -1;
                            },
                            CompareDescending = (a, b) => {
                                if (a?.Height > b?.Height)
                                    return -1;

                                if (a?.Height == b?.Height)
                                    return 0;

                                return 1;
                            },
                        }),
                    new TextColumn<Person, double>("Raw Height", person => person.Height),
                    new TextColumn<Person, Name.Gender>("Gender",
                        person => person.Gender), // To Template
                    new TextColumn<Person, decimal>("Money", person => person.Money),
                    new CheckBoxColumn<Person>("Checked", person => person.IsChecked),
                    new TextColumn<Person, string>("Email", person => person.Email),
                    new TextColumn<Person, string>("PhoneNumber", person => person.PhoneNumber),
                    new TextColumn<Person, string>("Address", person => person.Address),
                    new TextColumn<Person, string>("City", person => person.City),
                    new TextColumn<Person, string>("State", person => person.State),
                    new TextColumn<Person, string>("PostalCode", person => person.PostalCode),
                    new TextColumn<Person, string>("Country", person => person.Country),
                    new CheckBoxColumn<Person>("IsMarried", person => person.IsMarried),
                    new TextColumn<Person, DateTime?>("Anniv.", person => person.WeddingAnniversary),
                    new TemplateColumn<Person>("Hobbies", "HobbiesCell"),
                    new TemplateColumn<Person>("Languages", "LanguagesCell"),
                },
            };
    }

    public FlatTreeDataGridSource<Person> DataSource { get; set; }

    public string? FilterText {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    private static Func<Person, bool> BuildSearchFilter(string? text) {
        if (string.IsNullOrEmpty(text))
            return _ => true;

        return t => t.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            t.LastName.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    private static List<Person> GenerateFakes(int amount) {
        //Set the randomizer seed to generate repeatable data sets.
        Randomizer.Seed = new Random(8675309);
        var faker = new Faker<Person>().RuleFor(p => p.Id, faker => faker.IndexFaker)
            .RuleFor(p => p.DateOfBirth, faker => faker.Date.Past(80))
            .RuleFor(p => p.Height, faker => faker.Random.Double())
            .RuleFor(p => p.Gender, faker => faker.Person.Gender)
            .RuleFor(p => p.Money, faker => faker.Finance.Amount(-1000M, 1000M, 5))
            .RuleFor(p => p.IsChecked, faker => faker.Random.Bool())
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.Address, f => f.Address.StreetAddress())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.State, f => f.Address.State())
            .RuleFor(p => p.PostalCode, f => f.Address.ZipCode())
            .RuleFor(p => p.Country, f => f.Address.Country())
            .RuleFor(p => p.IsMarried, f => f.Random.Bool())
            .RuleFor(p => p.WeddingAnniversary,
                (f, p) => p.IsMarried ? f.Date.Past(10, p.DateOfBirth.AddYears(18)) : (DateTime?)null)
            .RuleFor(p => p.Hobbies,
                f => f.Make(3,
                    () => f.PickRandom("Fishing", "Cooking", "Gardening", "Reading", "Traveling", "Sports", "Art",
                        "Music")))
            .RuleFor(p => p.LanguagesSpoken, f => f.Make(2, () => f.Random.Word()));


        return faker.Generate(amount);
    }
}

public record Person {
    public int Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public double Height { get; set; }
    public Name.Gender Gender { get; set; }
    public decimal Money { get; set; }
    public bool IsChecked { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
    public bool IsMarried { get; set; }
    public DateTime? WeddingAnniversary { get; set; }
    public List<string> Hobbies { get; set; } = new();
    public List<string> LanguagesSpoken { get; set; } = new();
}
}
