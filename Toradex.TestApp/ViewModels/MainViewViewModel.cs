using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace Toradex.TestApp.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    private readonly ObservableCollection<int> _observableValues;
    private bool goUp = true;
    private int lastValue;

    public IEnumerable<ISeries> GaugeSeries { get; set; }
        = new GaugeBuilder()
            .WithLabelsSize(50)
            .WithInnerRadius(120)
            .WithBackgroundInnerRadius(120)
            .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
            .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
            .WithLabelFormatter(x => $"{x.PrimaryValue.ToString("N2")}%")
            .AddValue(30)
            .BuildSeries();
    
    public MainViewViewModel()
    {
        _observableValues = new ObservableCollection<int>(GenerateRandomStartUpValues());
        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<int>
            {
                Values = _observableValues,
                Fill = new SolidColorPaint(SKColor.Parse("#4DF0F0F0")),
                Stroke = new SolidColorPaint(SKColor.Parse("#3B87D7")) { StrokeThickness = 2 },
                GeometrySize = 0,
                LineSmoothness = 1
            }
        };

        if (Design.IsDesignMode)
            return;
        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                lastValue += goUp ? 1 : -1;
                _observableValues.Add(lastValue);
                this.RaisePropertyChanged(nameof(LastValue));
                goUp = lastValue switch
                {
                    100 => false,
                    0 => true,
                    _ => goUp
                };
                if (_observableValues.Count > 100)
                    _observableValues.RemoveAt(0);
                Thread.Sleep(1000);
            }
        });
    }

    public int LastValue => _observableValues.Count == 0 ? 0 : _observableValues[^1];

    public ObservableCollection<ISeries> Series { get; set; }

    private List<int> GenerateRandomStartUpValues()
    {
        var values = new List<int>();
        for (var i = 0; i < 100; i++)
        {
            lastValue += goUp ? 1 : -1;
            values.Add(lastValue);

            goUp = lastValue switch
            {
                100 => false,
                0 => true,
                _ => goUp
            };
        }

        return values;
    }
}