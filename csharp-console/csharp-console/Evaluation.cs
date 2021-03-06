﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace csharp_console
{
	public static class Evaluation
	{
		public static HttpClient client;
		const string baseAddress = "http://localhost:5000";

		static Evaluation()
		{
			client = new HttpClient();
			client.BaseAddress = new Uri(baseAddress);
		}

		public static double EuklidianDistance(PointD p1, PointD p2)
		{
			return Math.Sqrt((p1.X - p2.X)*(p1.X - p2.X) + (p1.Y - p2.Y)*(p1.Y - p2.Y));
		}
		public static async Task<double> RouteDistance(Warehouse wh, int routeIndex, Func<PointD, PointD, Task<double>> fitness)
		{
			if (routeIndex < 0 || routeIndex >= wh.CarsAmount)
				throw new IndexOutOfRangeException();

			double result = 0;
			if (wh.CarRoutes[routeIndex].Count > 0)
			{
				List<Task<double>> resultsTasks = new List<Task<double>>(wh.CarRoutes[routeIndex].Count);
				//result += await fitness(wh.Point, wh.CarRoutes[routeIndex][0]);
				resultsTasks.Add( fitness(wh.Point, wh.CarRoutes[routeIndex][0]) );
				for (int i = 1; i < wh.CarRoutes[routeIndex].Count; i++)
				{
					//result += await fitness(wh.CarRoutes[routeIndex][i-1], wh.CarRoutes[routeIndex][i]);
					resultsTasks.Add( fitness(wh.CarRoutes[routeIndex][i-1], wh.CarRoutes[routeIndex][i]) );
				}
				//result += await fitness(wh.CarRoutes[routeIndex][wh.CarRoutes[routeIndex].Count - 1], wh.Point);
				resultsTasks.Add( fitness(wh.CarRoutes[routeIndex][wh.CarRoutes[routeIndex].Count - 1], wh.Point) );

				double[] results = await Task.WhenAll(resultsTasks);
				result = results.Sum();
			}
			return result;
		}
		public static async Task<double> MapDistance(PointD p1, PointD p2)
		{
			const bool distance = false;
			string uri;
			if (distance)
				uri = $"/shortest/{p1};{p2}";
			else
				uri = $"/traveltime/{p1};{p2}";

			var msg = new HttpRequestMessage(HttpMethod.Get, uri);

			bool success = false;
			HttpResponseMessage response = null;
			while ( !success )
			{
				try
				{
					response = await client.SendAsync(msg);
					success = response.StatusCode == System.Net.HttpStatusCode.OK;
				}
				catch (Exception e)
				{
					Console.WriteLine("Server refused connection.", Console.Error);
					throw e;
				}
			}
			string content = await response.Content.ReadAsStringAsync();
			//try
			//{
			var json = JsonConvert.DeserializeObject<Dictionary<string, double>>(content);
			if (distance)
				return json["meters_distance"];
			else
				return json["travel_time"];

			//}
			//catch (Exception e)
			//{
			//	int a = 5;
			//	return 100_000;
			//}
		}
	}
}
