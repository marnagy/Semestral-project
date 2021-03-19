# The algorithm

- client program uses Evolutionary Strategies (ES) to find the best solution
- client runs serveral times and saves best solution from each run to output directory
- 
## Mutations
### Route mutation
- swaps 2 points on a random route of length at least 2 of a random warehouse
- accepts if the changed warehouse has better fitness, else returns to previous state

### Warehouse mutation
- moves warehouse randomly using Gaussian distribution using predefined 'sigma' variable for both dimensions
- accepts if change has better fitness, else returns to previous state

### Point-warehouse mutation
- moves point from one warehouse to another warehouse
- accepts if change has better fitness, else returns to previous state

# How to run

## REST API part
- move to directory 'python'
- start REST API
- after running the client part, interrupt program using Ctrl+C

## Client part
- move to directory 'csharp-console'
- start project 'csharp-console' using dotnet (use Release configuration for max performance)
- this program will write stats about each generation to output
- [WIP] configuration from file?, hard-coded values for now
- saves solution for each run to output directory with extension '.wh'

## Visualization part
- move to directory 'rest'
- run 'visualization.py' with first argument being path to directory containing solutions from Client part
- this program creates new folder in output folder and saves PDF visualization for each solution

## Requirements
- dotnet or runtime that can run C# 8.0+ (tested using dotnet)
- python 3.9+
- installed python modules listed in 'required.txt' in 'rest' directory



## csharp-console directory

- expects server listening on 'http://localhost:5000'
- Stores solution of C# client implementation using asynchronous programming for maximum speed
- designed for communicate with 'flask_rest.py' from 'rest' directory (getting distance/time)
- [WIP] outputs to directory "csharp_results" using full path
- implements Evolutionary algorithm using Evolutionary Strategies

## python directory

- Stores scripts that use osmnx module

- This includes: Flask REST API (flask_rest.py), script for visualization of computed result (visualization.py) and script for downloading needed (Prague) map (download_map.py)

### flask_rest.py

- expects 'prague.osm' and 'visualization.py' files in current directory

### visualization.py

- expects 'prague.osm' file in current directory

### download_map.py

- used to download map and save in current directory
- [WIP] hard-coded to download Prague map
- saves map to current directory to file 'prague.osm'

### addresses_overpass.py

- get n shops in Prague using: python addresses_overpass.py --amount=n > [output file]
- for generating test input