from overpy import Overpass, Result
import random
from typing import List, Tuple
from argparse import ArgumentParser, Namespace

def get_args() -> Namespace:
    """Get arguments from CLI.

    :return: Needed arguments.
    :rtype: Namespace
    """
    parser = ArgumentParser()
    parser.add_argument("--amount", default=20, type=int, help="Amount of shops to get.")

    args = parser.parse_args(None)
    return args

def get_query_result(place_name: str) -> Result:
    """Get result from Overpass API

    :param place_name: Name of place to get shops from
    :type place_name: str
    :return: Overpass query result
    :rtype: Result
    """
    api = Overpass()

    query_str = '''area[name="{}"];
        node[shop="supermarket"](area);
        out;'''.format(place_name)

    result = api.query(query_str)
    return result

def result_to_coords(result: Result) -> List[Tuple[float, float]]:
    """Get simple coords from Overpass query result

    :param result: Overpass query result
    :type result: Result
    :return: Simple coords in tuple
    :rtype: List[Tuple[float, float]]
    """
    nodes_lat_lon = list(map(lambda x: (float(x.lat), float(x.lon)), result.nodes))
    return nodes_lat_lon

def print_coords(coords: List[Tuple[float, float]], args: Namespace):
    """Print coords in predefined format to output.

    :param coords: List of coord tuples.
    :type coords: List[Tuple[float, float]]
    :param amount: Max amount of coords to save.
    :type amount: int
    """
    if len(coords) > args.amount:
        print(args.amount)
        for node in random.choices(coords, k=args.amount):
            print("{};{}".format(node[0],node[1]))
    else:
        print(len(coords))
        for node in coords:
            print("{};{}".format(node[0],node[1]))

def main():
    args = get_args()
    result = get_query_result("Praha")
    coords = result_to_coords(result)
    print_coords(coords, args)

if __name__ == "__main__":
    main()