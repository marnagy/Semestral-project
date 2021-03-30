﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace csharp_console
{
	public class Config
	{
		public int WarehousesAmount { get; set; }
		public int[] CarsAmount { get; set; }
		public int NGen { get; set; }
		public int PopulationSize { get; set; }
		public double WarehouseMutProb { get; set; }
		public double PointWarehouseMutProb { get; set; }
		public double RouteMutProb { get; set; }
		public int Runs { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    
	}
}
