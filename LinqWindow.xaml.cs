using LoomamaaApp.Klassid;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LoomamaaApp
{
    public partial class LinqWindow : Window
    {
        private readonly IEnumerable<Animal> animals;
        private readonly IEnumerable<Enclosure<Animal>> enclosures;

        public LinqWindow(IEnumerable<Animal> animals, IEnumerable<Enclosure<Animal>> enclosures)
        {
            InitializeComponent();
            this.animals = animals ?? new List<Animal>();
            this.enclosures = enclosures ?? new List<Enclosure<Animal>>();
            RunQueries();
        }

        private void RunQueries()
        {
            // Query 1: GroupBy type -> count and average age
            var q1 = animals
                .GroupBy(a => a.TypeName)
                .Select(g => new GroupResult
                {
                    Type = g.Key,
                    Count = g.Count(),
                    AvgAge = g.Any() ? g.Average(a => a.Age) : 0
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            GroupGrid.ItemsSource = q1;

            // Query 2: Oldest animal per type
            var q2 = animals
                .GroupBy(a => a.TypeName)
                .Select(g => g.OrderByDescending(a => a.Age).First())
                .OrderByDescending(a => a.Age)
                .Select(a => $"{a.TypeName}: {a.Name} ({a.Age} years)")
                .ToList();

            OldestListBox.ItemsSource = q2;

            // Query 3: Animals older than overall average age
            var avgAll = animals.Any() ? animals.Average(a => a.Age) : 0;
            var q3 = animals
                .Where(a => a.Age > avgAll)
                .OrderByDescending(a => a.Age)
                .Select(a => $"{a.TypeName}: {a.Name} ({a.Age} years)")
                .ToList();

            AboveAvgListBox.ItemsSource = q3;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RunQueries();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private class GroupResult
        {
            public string Type { get; set; }
            public int Count { get; set; }
            public double AvgAge { get; set; }
        }
    }
}
