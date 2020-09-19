import random

import numpy

from deap import algorithms
from deap import base
from deap import creator
from deap import tools

IND_INIT_SIZE = 5
MAX_ITEM = 50
MAX_WEIGHT = 50
NBR_ITEMS = 200

# To assure reproductibility, the RNG seed is set prior to the items
# dict initialization. It is also seeded in main().
random.seed(64)

# Create the item dictionary: item name is an integer, and value is 
# a (weight, value) 2-uple.
items = {}
# Create random items and store them in the items' dictionary.
for i in range(NBR_ITEMS):
	items[i] = (random.randint(1, 10), random.uniform(0, 100))

#creator.create("Fitness", base.Fitness, weights=(-1.0, 1.0))
creator.create("Fitness", base.Fitness, weights=(-1.0, 1.0))
creator.create("Individual", set, fitness=creator.Fitness)

toolbox = base.Toolbox()
# Attribute generator
toolbox.register("attr_item", random.randrange, NBR_ITEMS)
# Structure initializers
toolbox.register("individual", tools.initRepeat, creator.Individual, 
	toolbox.attr_item, IND_INIT_SIZE)
toolbox.register("population", tools.initRepeat, list, toolbox.individual)

def evalKnapsack(individual):
	weight = 0.0
	value = 0.0
	for item in individual:
		weight += items[item][0]
		value += items[item][1]
	if len(individual) > MAX_ITEM or weight > MAX_WEIGHT:
		return 10000, 0             # Ensure overweighted bags are dominated
	return weight, value

def cxSet(ind1, ind2):
	"""Apply a crossover operation on input sets. The first child is the
	intersection of the two sets, the second child is the difference of the
	two sets.
	"""
	temp = set(ind1)                # Used in order to keep type
	ind1 &= ind2                    # Intersection (inplace)
	ind2 ^= temp                    # Symmetric Difference (inplace)
	return ind1, ind2
	
def mutSet(individual):
	"""Mutation that pops or add an element."""
	if random.random() < 0.5:
		if len(individual) > 0:     # We cannot pop from an empty set
			individual.remove(random.choice(sorted(tuple(individual))))
	else:
		individual.add(random.randrange(NBR_ITEMS))
	return individual,

toolbox.register("evaluate", evalKnapsack)
toolbox.register("mate", cxSet)
toolbox.register("mutate", mutSet)
toolbox.register("select", tools.selNSGA2)

def main():
	random.seed(64)
	NGEN = 50 # amount of generations
	MU = 50 # amount of individuals to select from each generation (possible parents)
	LAMBDA = 150 # number of new children for each generation
	CXPB = 0.7 # probability of mating
	MUTPB = 0.2 # mutation probability
	
	pop = toolbox.population(n=MU)
	hof = tools.ParetoFront()
	stats = tools.Statistics(lambda ind: ind.fitness.values)
	stats.register("avg", numpy.mean, axis=0)
	stats.register("std", numpy.std, axis=0)
	stats.register("min", numpy.min, axis=0)
	stats.register("max", numpy.max, axis=0)
	
	# algorithms.eaSimple(pop, toolbox, CXPB, MUTPB, NGEN,
	# 					stats, halloffame=hof)

	algorithms.eaMuPlusLambda(pop, toolbox, MU, LAMBDA, CXPB, MUTPB, NGEN,
								stats, halloffame=hof)
	
	#algorithms.eaMuCommaLambda(pop, toolbox, MU, LAMBDA, CXPB, MUTPB, NGEN, stats, hof)

	return pop, stats, hof
				 
if __name__ == "__main__":
	_, _, hof = main()
	print(hof.items)