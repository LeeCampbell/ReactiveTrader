<Query Kind="Program">
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Client.DomainPortable.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Client.DomainPortable.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Shared.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Adaptive.ReactiveTrader.Shared.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\HdrHistogram.NET.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\HdrHistogram.NET.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Microsoft.AspNet.SignalR.Client.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Microsoft.AspNet.SignalR.Client.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Newtonsoft.Json.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\Newtonsoft.Json.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Net.Http.Extensions.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Net.Http.Extensions.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Net.Http.Primitives.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Net.Http.Primitives.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Core.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Core.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Interfaces.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Interfaces.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Reference Relative="Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.PlatformServices.dll">&lt;MyDocuments&gt;\GitHub\ReactiveTrader\src\Adaptive.ReactiveTrader.Client.Domain\bin\Debug\System.Reactive.PlatformServices.dll</Reference>
  <Namespace>Adaptive.ReactiveTrader.Client.Domain</Namespace>
  <Namespace>Adaptive.ReactiveTrader.Client.Domain.Models</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

void Main()
{
	var api = new ReactiveTrader();
	
	//Initialize
	api.Initialize("Trader1", new []{"http://localhost:8080"});
	
	
	//Dump connection status
	api.ConnectionStatusStream.DumpLive("Conn");
	
	//Get EUR/USD price stream
	var eurusd = api.ReferenceData.GetCurrencyPairsStream()
					.SelectMany(ccyList=>ccyList)
					.Where(ccy=>ccy.CurrencyPair.Symbol == "EURUSD")
					.SelectMany(ccy=>ccy.CurrencyPair.PriceStream)
				 	.Publish()
					.RefCount();
	
	
	//Dump prices to live
	eurusd.DumpLive("Prices");

	//Execute
	var maxBidRate = 1.3625m;
	var order = eurusd.Where(p=>p.Bid.Rate < maxBidRate)
					.Take(1)
					.SelectMany(price=>price.Bid.ExecuteRequest(100000, "EUR"))
					.Select(trade=> trade.Update.TradeStatus);
				
	order.DumpLive("Order");					
}