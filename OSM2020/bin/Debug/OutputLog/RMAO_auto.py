# -*- coding: utf-8 -*-
import re
import pandas as pd
import numpy as np
import glob
from statistics import mean
import matplotlib.pyplot as plt
import os
import pathlib
from tqdm import tqdm
import sys

def main():
    argv = sys.argv
    argc = len(argv)
    print(argv)
    if(argc != 2):
        print('[Python] ' + 'Arg Error')
        quit()
    file_name = argv[1]
    GraphicalData(file_name)

def GraphicalData(file_name):
    print("グラフを出力するよ")
    #custom_round_set = set([200])

    common_output_dir = 'output'
    #os.chdir(file_name)
    #os.chdir('data')

    #p = pathlib.Path('./' + file_name + '/data/')
    p = "C:/Users/nekko/Documents/TakadamaLab/OSM/OSM2020/OSM2020/bin/Debug/OutputLog/" + file_name + "/data/"
    files = os.listdir(p)
    print(files)
    #dir_list = [i for i in p.iterdir() if i.is_dir()]
    #print(dir_list)
    for d in tqdm(files):
        #if str(d) == common_output_dir:
            #continue
        #os.chdir(d) #Change Directory.これでCSVが出力されるファイルの中に入る
        csv_path = p + d + "/"
        print(csv_path)

        csv_list = glob.glob(csv_path + "*.csv")
        print(csv_list)
        folder_path = csv_path
        folder_name = d
        fig_width = 12
        fig_height = 10

        for csv in csv_list:
            df = pd.read_csv(csv)
            custom_round_set = df['Round'].tail(1)
            for c_round_num in custom_round_set:
                x_tick_start = 0
                x_tick_end = df['Round'][0:c_round_num].max() + 10
                x_tick_dur = 100
                x_label = "Round, $m$"
                #colorlist = ["g", "r", "darkgrey"]
                seed = str(csv).split('_')[1]
                title_name = csv_path + folder_name + seed
                title_name_bad = csv_path + 'Bad_' + folder_name + seed
                title_name_nor = csv_path + 'Nor_' + folder_name + seed

                ax = df[['CorrectRate', 'IncorrectRate', 'UndeterRate']][0:c_round_num].plot(kind = 'area', 
                    figsize=(fig_width, fig_height),
                    #color=colorlist
                    linewidth=0.05, alpha=1.0
                    )
                plt.xticks(np.arange(x_tick_start, x_tick_end, x_tick_dur))
                plt.title(folder_name + seed)
                plt.yticks(np.arange(0, 1.05, 0.2))
                plt.legend(fontsize=24)
                #plt.xlabel("Dim", fontsize=24)
                plt.xlabel(x_label, fontsize=32)
                plt.ylabel("Rate", fontsize=32)
                plt.tick_params(labelsize=32)
                plt.tight_layout()
                plt.savefig(title_name + '_' + str(c_round_num) + '.png')
                plt.close('all')

                ax = df[['BadCommunityCorrectRate', 'BadCommunityIncorrectRate', 'BadCommunityUndeterRate']][0:c_round_num].plot(kind = 'area', 
                    figsize=(fig_width, fig_height),
                    #color=colorlist
                    linewidth=0.05, alpha=1.0
                    )
                plt.xticks(np.arange(x_tick_start, x_tick_end, x_tick_dur))
                plt.title('Bad_' + folder_name + seed)
                plt.yticks(np.arange(0, 1.05, 0.2))
                plt.legend(fontsize=24)
                #plt.xlabel("Dim", fontsize=24)
                plt.xlabel(x_label, fontsize=32)
                plt.ylabel("Rate", fontsize=32)
                plt.tick_params(labelsize=32)
                plt.tight_layout()
                plt.savefig(title_name_bad + '_' + str(c_round_num) + '.png')
                plt.close('all')

                ax = df[['NormalCommunityCorrectRate', 'NormalCommunityIncorrectRate', 'NormalCommunityUndeterRate']][0:c_round_num].plot(kind = 'area', 
                    figsize=(fig_width, fig_height),
                    #color=colorlist
                    linewidth=0.05, alpha=1.0
                    )
                plt.xticks(np.arange(x_tick_start, x_tick_end, x_tick_dur))
                plt.title('Nor_' + folder_name + seed)
                plt.yticks(np.arange(0, 1.05, 0.2))
                plt.legend(fontsize=24)
                #plt.xlabel("Dim", fontsize=24)
                plt.xlabel(x_label, fontsize=32)
                plt.ylabel("Rate", fontsize=32)
                plt.tick_params(labelsize=32)
                plt.tight_layout()
                plt.savefig(title_name_nor + '_' + str(c_round_num) + '.png')
                plt.close('all')
        #os.chdir('../')

if __name__ == "__main__":
    main()
