﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_console.Mutations
{
	class ChangeWarehouseOfPoint
	{
		private static readonly Random rand = new Random();
		public async static Task SimpleChange(WarehousesChromosome whc)
		{
			if (whc.warehouses.Length == 1) return;

			double oldFitness = whc.Fitness;

			List<int> nonEmptyWHIndices = new List<int>();
			for (int i = 0; i < whc.warehouses.Length; i++)
			{
				var wh = whc.warehouses[i];
				if ( wh.CarRoutes.Any(route => route.Count > 0))
				{
					nonEmptyWHIndices.Add(i);
				}
			}

			// if (nonEmptyWHIndices.Count < 2)
			// 	return;

			int fromWHIndex = rand.Next(nonEmptyWHIndices.Count);
			var whFrom = whc.warehouses[nonEmptyWHIndices[fromWHIndex]];
			var whTo = whc.warehouses[ rand.Next( whc.warehouses.Length ) ];
			while ( whFrom == whTo )
			{
				whTo = whc.warehouses[ rand.Next( whc.warehouses.Length ) ];
			}
			//nonEmptyWHIndices.Clear();
			double fromOldFitness = whFrom.Fitness;
			double toOldFitness = whTo.Fitness;

			double whFromOldFitness = whFrom.Fitness;
			double whToOldFitness = whTo.Fitness;

			int routeIndexFrom = GetRouteIndexFrom(whFrom);
			int routeIndexTo = GetRouteIndexTo(whTo);

			int pointIndexFrom = rand.Next(whFrom.CarRoutes[routeIndexFrom].Count);
			int pointIndexTo = rand.Next(whTo.CarRoutes[routeIndexTo].Count + 1);

			{
				PointD point = whFrom.CarRoutes[routeIndexFrom][pointIndexFrom];
				whFrom.CarRoutes[routeIndexFrom].RemoveAt(pointIndexFrom);
				whTo.CarRoutes[routeIndexTo].Insert(pointIndexTo, point);
			}

			double fromNewFitness = await whFrom.ComputeDistanceAndSave();
			double toNewFitness = await whTo.ComputeDistanceAndSave();
			//whc.UpdateFitness();
			if ( Max(fromNewFitness, toNewFitness) < Max(fromOldFitness, toOldFitness) )
			{
				whc.UpdateFitness();
				//whc.ChangeWarehouseFitness(index: fromWHIndex, fromOldFitness, fromNewFitness);
				//whc.ChangeWarehouseFitness(index: toWHIndex, toOldFitness, toNewFitness);
				return;
			}
			else
			{
				// change back
				PointD point = whTo.CarRoutes[routeIndexTo][pointIndexTo];
				whTo.CarRoutes[routeIndexTo].RemoveAt(pointIndexTo);
				whFrom.CarRoutes[routeIndexFrom].Insert(pointIndexFrom, point);

				whTo.Fitness = toOldFitness;
				whFrom.Fitness = fromOldFitness;

				//whc.ChangeWarehouseFitness(index: fromWHIndex, fromNewFitness, toOldFitness);
				//whc.ChangeWarehouseFitness(index: toWHIndex, toNewFitness, fromOldFitness);
			}
		}
		private static double Max(double d1, double d2)
		{
			if (d1 > d2)
				return d1;
			else
				return d2;
		}

		private static int GetRouteIndexTo(Warehouse whTo)
		{
			// maybe prefer routes with closer points?
			// random for now
			return rand.Next(whTo.CarRoutes.Length);
		}

		private static int GetRouteIndexFrom(Warehouse whFrom)
		{
			List<int> indices = new List<int>();
			for (int i = 0; i < whFrom.CarRoutes.Length; i++)
			{
				if (whFrom.CarRoutes[i].Count > 0)
					indices.Add(i);
			}
			return indices[rand.Next(indices.Count)];
		}
	}
}
