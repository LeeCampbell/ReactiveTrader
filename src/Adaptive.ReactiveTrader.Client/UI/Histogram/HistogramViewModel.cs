using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Adaptive.ReactiveTrader.Client.Domain.Instrumentation;
using Adaptive.ReactiveTrader.Client.UI.SpotTiles;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.Histogram
{
    public class HistogramViewModel : ViewModelBase, IHistogramViewModel
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly EventLoopScheduler _eventLoopScheduler = new EventLoopScheduler();
        private readonly Func<ITextFileWriter> _gnuPlotFactory;
        private int _index = 0;
        
        public HistogramViewModel(Func<ITextFileWriter> gnuPlotFactory)
        {
            _gnuPlotFactory = gnuPlotFactory;
        }

        public void OnStatistics(Statistics statistics, SpotTileSubscriptionMode subscriptionMode)
        {
            _eventLoopScheduler.Schedule(() => Render(statistics, subscriptionMode));
        }

        private void Render(Statistics statistics, SpotTileSubscriptionMode subscriptionMode)
        {
            var gnuPlot = _gnuPlotFactory();
            gnuPlot.WriteFile(string.Format(@".\output-{0}-{1}-{2}.hgrm", _index, subscriptionMode, _stopwatch.Elapsed.ToString().Replace(':', '.')),
                statistics.Histogram);
            _stopwatch.Restart();
            _index++;
        }
       
        public string ImageSource { get; private set; }
    }
}