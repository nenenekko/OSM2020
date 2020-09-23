import sys
import networkx as nx
import matplotlib.pyplot as plt
import pandas as pd
import os
from networkx.readwrite import json_graph
from networkx.generators.community import LFR_benchmark_graph
import json

def main():
    argv = sys.argv
    argc = len(argv)
    print(argv)
    if(argc != 2):
        print('[Python] ' + 'Arg Error')
        quit()
    n = int(argv[1])
    generate_graph(n)
    print('[Python]' + 'Generate Graph')

def generate_graph(n):
    tau1 = 3.2
    tau2 = 1.4
    mu = 0.1
    G = LFR_benchmark_graph(n, tau1, tau2, mu, average_degree=5, min_community=20,max_iters=1000,seed=10)
    communities = {frozenset(G.nodes[v]['community']) for v in G}

    A = nx.to_numpy_matrix(G)
    G = nx.from_numpy_matrix(A)
    data = json_graph.node_link_data(G)
    with open('./Working/graph.json', 'w') as f:
        json.dump(data, f)
    with open('./Working/graph_flag', mode = 'w', encoding = 'utf-8') as fh:
        fh.write("i look at you")

    
    print(len(list(communities)))
    with open('./Working/community.txt', 'w') as f:
      for i in range(len(list(communities))):
        community_people = list(list(communities)[i])
        for j in range(len(community_people)):
            print(community_people[j], file=f)
            if j != (len(community_people) - 1):
                print(',', file=f)
        print('/',file=f)

    with open('./Working/community.txt') as f:
      print(f.readlines())

if __name__ == '__main__':
    main()