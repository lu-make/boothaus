using System.Collections.ObjectModel;

namespace Boothaus.GUI.ViewModels;

public static class CollectionExtensions
{
    public static void Update<T>(this ObservableCollection<T> target, IEnumerable<T> source) 
    {
        var neu = source.Except(target).ToList();
        var weg = target.Except(source).ToList();

        foreach (var item in neu)
        {
            target.Add(item);
        }

        foreach (var item in weg)
        {
            target.Remove(item);
        }
    }
}