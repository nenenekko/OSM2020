import pandas as pd
import matplotlib.pyplot as plt
import glob
import pathlib
import os
import numpy as np
from tqdm import tqdm
import matplotlib.ticker as ticker
import warnings
import math
import seaborn as sns; sns.set()
warnings.filterwarnings('ignore')

graph_set = set()
size_set = set()
algorithm_set = set()
th_set = set()
dim_set = set()
dynamic_set = set()
env_set = set()
env_dist_weight_set = set()
info_weight_rate_set = set()
round_set = set()
sensor_size_rate_set = set()
mal_sensor_size_rate_set = set()
common_weight_set = set()

seed_set = set()
badmode_set = set()
share_num_set = set()



x_column = 'th'
y_column = 'round_correct'
custom_round_set = set([200])

#hensuu
common_output_dir = 'output'
output_dir = common_output_dir + '/' + x_column + '/'



if not os.path.exists(common_output_dir):
    os.mkdir(common_output_dir)

if not os.path.exists(output_dir):
    os.mkdir(output_dir)

os.chdir('data')
p = pathlib.Path('./')
dir_list = [i for i in p.iterdir() if i.is_dir()]
filename_list = []
for d in dir_list:
    if str(d) == common_output_dir:
        continue
    print(str(d))
    seed_set.add(str(d).split('_')[0])
    graph_set.add(str(d).split('_')[1]) #0
    badmode_set.add(str(d).split('_')[2])
    size_set.add(int(str(d).split('_')[3])) #1
    share_num_set.add(str(d).split('_')[4])
    algorithm_set.add(str(d).split('_')[5]) #2
    #dim_set.add(int(str(d).split('_')[3]))
    #dynamic_set.add(str(d).split('_')[4])
    th_set.add(float(str(d).split('_')[6])) #5
    env_set.add(str(d).split('_')[7]) #6
    #env_dist_weight_set.add(float(str(d).split('_')[7]))
    info_weight_rate_set.add(str(d).split('_')[8])
    round_set.add(int(str(d).split('_')[9]))
    sensor_size_rate_set.add(float(str(d).split('_rate')[1].split('_')[0]))
    #mal_sensor_size_rate_set.add(float(str(d).split('_')[11]))
    common_weight_set.add(float(str(d).split('_')[11]))

    filename_list.append(d)

set_dic = {'graph': graph_set, 
            'size': size_set, 
            'algo': algorithm_set, 
            'th': th_set, 
            #'dim': dim_set, 
            #'dynamic': dynamic_set, 
            'env': env_set, 
            #'env_dist_weight': env_dist_weight_set, 
            'info_weight_rate': info_weight_rate_set, 
            'round': round_set, 
            'sensor_size_rate': sensor_size_rate_set, 
            #'mal_sensor_size_rate': mal_sensor_size_rate_set,
            'cw': common_weight_set,
            'badmode': badmode_set
            }


#plot
x_tick_start = 0
x_tick_end = 0
x_tick_dur = 0
x_label = ""
if x_column == 'graph':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'size':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = "Number of Agents, $N_A$"
elif x_column == 'algo':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'th':
    x_tick_start = 0.5
    x_tick_end = max(set_dic[x_column]) + 0.01
    x_tick_dur = 0.1
    x_label = "Target awareness rate, $h_{target}$"
elif x_column == 'dim':
    x_tick_start = 2
    x_tick_end = max(set_dic[x_column]) + 1
    x_tick_dur = 1
    x_label = "Number of opinion state, $n$"
elif x_column == 'dynamic':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'env':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'env_dist_weight':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 0.1
    x_tick_dur = 0.1
    x_label = "Environment Distribution Weight"
elif x_column == 'info_weight_rate':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'round':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = "Round, $m$"
elif x_column == 'sensor_size_rate':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'mal_sensor_size_rate':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = ""
elif x_column == 'cw':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 0.1
    x_tick_dur = 0.1
    x_label = "Common weight, $w_c$"
elif x_column == 'badmode':
    x_tick_start = 0.0
    x_tick_end = max(set_dic[x_column]) + 100
    x_tick_dur = 200
    x_label = "BadMode, $bm$"

fig_width = 8
fig_height = 6

colum_list = ['graph',
              'size',
              'algo',
              #'dim',
              'th',
              'seed',
              'round_correct',
              #'dynamic',
              'env',
              #'env_dist_weight',
              'info_weight_rate',
              'round',
              'sensor_size_rate',
              #'mal_sensor_size_rate',
              'cw',
              'badmode'
              ]

df_th = pd.DataFrame(columns = colum_list)

loop_num = 1
for each_set in set_dic:
    loop_num *= len(set_dic[each_set])

with tqdm(total=loop_num) as pbar:
    for seed in seed_set:
        for graph in graph_set:
            for badmode in badmode_set:
                for size in size_set:
                    for sharenum in share_num_set:
                        for algo in algorithm_set:
                            #for dim in dim_set:
                                for th in th_set:
                                    #for dynamic in dynamic_set:
                                        for env in env_set:
                                            #for env_dist_weight in env_dist_weight_set:
                                                for info_weight_rate in info_weight_rate_set:
                                                    for round_num in round_set:
                                                        if round_num != max(round_set):
                                                            continue

                                                        for sensor_size_rate in sensor_size_rate_set:
                                                            #for mal_sensor_size_rate in mal_sensor_size_rate_set:
                                                                for cw in common_weight_set:
                                                                        pbar.update(1)
                                                                        if th == 1.0:
                                                                            th = 1
                                                                        if th == 0.0:
                                                                            th = 0
                                                                        """
                                                                        if env_dist_weight == 1.0:
                                                                            env_dist_weight = 1
                                                                        if env_dist_weight == 0.0:
                                                                            env_dist_weight = 0
                                                                        """
                                                                        if info_weight_rate == 1.0:
                                                                            info_weight_rate = 1
                                                                        if info_weight_rate == 0.0:
                                                                            info_weight_rate = 0

                                                                        if sensor_size_rate == 1.0:
                                                                            sensor_size_rate = 1
                                                                        if sensor_size_rate == 0.0:
                                                                            sensor_size_rate = 0
                                                                        """
                                                                        if mal_sensor_size_rate == 1.0:
                                                                            mal_sensor_size_rate = 1
                                                                        if mal_sensor_size_rate == 0.0:
                                                                            mal_sensor_size_rate = 0
                                                                        """
                                                                        if cw == 1.0:
                                                                            cw = 1
                                                                        if cw == 0.0:
                                                                            cw = 0

                                                                        target_str = seed + '_' + graph + '_' + badmode + '_' + str(size) + '_' + sharenum + '_' + algo + '_' + str(th) + '_' + str(env) + '_' + str(info_weight_rate) + '_' + str(round_num) + '_rate' + str(sensor_size_rate) + '_' + str(cw)
                                                                                #+ str(dim) + '_'
                                                                                #+ str(dynamic) + '_'
                                                                                #+ str(env_dist_weight) + '_'
                                                                                #+ str(mal_sensor_size_rate) + '_'
                                                                        target_path = p.glob(target_str)
                                                                        print(target_str)
                                                                        target_dir = list(target_path)
                                                                        print(target_dir)
                                                                        if len(target_dir) > 1:
                                                                            warnings.warn("duplication error")
                                                                        if len(target_dir) == 0:
                                                                            warnings.warn("not equal error")
                                                                            continue
                                                                        csv_list = list(target_dir[0].glob('*.csv'))
                                                                        print(csv_list)
                                                                        for csv in csv_list:
                                                                            #seed = str(csv).split('_')[-2]
                                                                            df_csv_raw = pd.read_csv(str(csv))

                                                                            for c_round_num in custom_round_set:
                                                                                df_csv = df_csv_raw[0:c_round_num]

                                                                                #df_acc_raw = df_csv['IncorrectRate']
                                                                                #df_acc_raw = df_csv['CorrectRate']

                                                                                #ave_cor = round(df_csv['CorrectRate'][-100::].mean(), 3)
                                                                                #ave_incor = round(df_csv['IncorrectRate'][-100::].mean(), 3)
                                                                                #inv_ave_incor = 1 - ave_incor
                                                                                #inv_ave_incor = inv_ave_incor ** math.e
                                                                                #ave_acc = inv_ave_incor * ave_cor

                                                                                df_acc_raw = (df_csv['CorrectRate'] / (df_csv['CorrectRate'] + df_csv['IncorrectRate'])) * (1 - df_csv['UndeterRate'])
                                                                                df_nor_acc_raw = (df_csv['NormalCommunityCorrectRate'] / (df_csv['NormalCommunityCorrectRate'] + df_csv['NormalCommunityIncorrectRate'])) * (1 - df_csv['NormalCommunityUndeterRate'])
                                                                                df_bad_acc_raw = (df_csv['BadCommunityCorrectRate'] / (df_csv['BadCommunityCorrectRate'] + df_csv['BadCommunityIncorrectRate'])) * (1 - df_csv['BadCommunityUndeterRate'])
                                                                                #ave_acc = round(((df_csv['CorrectRate'] / (df_csv['CorrectRate'] + df_csv['IncorrectRate'])) * (1 - df_csv['UndeterRate'])).mean(),3)
                                                                                #ave_acc = round(df_csv['CorrectRate'].mean(), 3)
                                                                                ave_acc = round(df_acc_raw.mean(), 3)
                                                                                ave_nor_acc = round(df_nor_acc_raw.mean(), 3)
                                                                                ave_bad_acc = round(df_bad_acc_raw.mean(), 3)
                                                                                #ave_acc = round(df_acc_raw.mean()**math.exp(-1), 3)
                                                                                #ave_acc = round(df_acc_raw.apply(lambda x: math.pow(100, x)).sum(), 3)

                                                                                #cor_incor_comp = (df_csv['CorrectRate'] - df_csv['IncorrectRate']).apply(lambda x: math.exp(x))
                                                                                #cor_und_comp = (df_csv['CorrectRate'] - df_csv['UndeterRate']).apply(lambda x: math.exp(x))
                                                                                #round_acc = cor_incor_comp * cor_und_comp
                                                                                #ave_acc = round(round_acc.mean(), 3)

                                                                                #ave_acc = (df_csv['CorrectRate'][-150::] > 1/2).sum() / df_csv[-150::].shape[0]
                                                                                #ave_acc = (df_csv['CorrectRate'] > df_csv['IncorrectRate'])[-150::].sum() / df_csv[-150::].shape[0]
                                                                                #und_acc = (df_csv['CorrectRate'] > df_csv['UndeterRate'])[-150::].sum() / df_csv[-150::].shape[0]
                                                                                #ave_acc = ave_acc * und_acc
                                                                                #tmp_df = df_csv[-150::]
                                                                                #tmp_df2 = tmp_df[(tmp_df['CorrectRate'] > tmp_df['IncorrectRate'])]
                                                                                #ave_acc = (tmp_df2['CorrectRate'] > tmp_df2['UndeterRate']).sum() / tmp_df.shape[0]
                                                                                #win_rate = 1
                                                                                #ave_acc = win_rate * df_csv[(df_csv['CorrectRate'] > df_csv['IncorrectRate'])]['CorrectRate'][-150::].mean()

                                                                                #ave_acc = round(df_csv['CorrectRate'].mean() * (1 - df_csv['CorrectRate'].std()), 3)

                                                                                df_th = df_th.append(pd.Series([graph, 
                                                                                                                size,
                                                                                                                algo,
                                                                                                                #dim,
                                                                                                                th,
                                                                                                                seed,
                                                                                                                ave_acc,
                                                                                                                #dynamic,
                                                                                                                env, 
                                                                                                                #env_dist_weight, 
                                                                                                                info_weight_rate,
                                                                                                                c_round_num, 
                                                                                                                sensor_size_rate, 
                                                                                                                #mal_sensor_size_rate,
                                                                                                                cw,
                                                                                                                badmode
                                                                                                                ], index = colum_list), ignore_index = True)
                                                                                df_th_nor = df_th.append(pd.Series([graph, 
                                                                                                                size,
                                                                                                                algo,
                                                                                                                #dim,
                                                                                                                th,
                                                                                                                seed,
                                                                                                                ave_nor_acc,
                                                                                                                #dynamic,
                                                                                                                env, 
                                                                                                                #env_dist_weight, 
                                                                                                                info_weight_rate,
                                                                                                                c_round_num, 
                                                                                                                sensor_size_rate, 
                                                                                                                #mal_sensor_size_rate,
                                                                                                                cw,
                                                                                                                badmode
                                                                                                                ], index = colum_list), ignore_index = True)
                                                                                df_th_bad = df_th.append(pd.Series([graph, 
                                                                                                                size,
                                                                                                                algo,
                                                                                                                #dim,
                                                                                                                th,
                                                                                                                seed,
                                                                                                                ave_bad_acc,
                                                                                                                #dynamic,
                                                                                                                env, 
                                                                                                                #env_dist_weight, 
                                                                                                                info_weight_rate,
                                                                                                                c_round_num, 
                                                                                                                sensor_size_rate, 
                                                                                                                #mal_sensor_size_rate,
                                                                                                                cw,
                                                                                                                badmode
                                                                                                                ], index = colum_list), ignore_index = True)

for hue_column in set_dic:
    if hue_column == x_column:
        continue

    group_column_list = [c for c in colum_list if (c != 'seed' and c != y_column and c != x_column and c != hue_column) ]
    groups = df_th.groupby(group_column_list)
    groups_nor = df_th_nor.groupby(group_column_list)
    groups_bad = df_th_bad.groupby(group_column_list)
    i=0
    for ((name, group),(name_nor,group_nor),(name_bad,group_bad)) in zip(groups,groups_nor,groups_bad):

        if group[hue_column].unique().size <= 1:
            continue
        #if hue_column != 'algo':
        #    continue
        #group = group.sort_values('algo', ascending=True)

        plt.figure(figsize=(fig_width, fig_height))
        title_column = [c for c in group.columns if (c != 'seed' and c != y_column and c != x_column and c != hue_column) ]
        title_name = map(str, name)
        title_name = '_'.join(title_name)
        ax = None
        if hue_column == 'graph':
            print(group)
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("Set1", len(group[hue_column].unique())),
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("Set1", len(group[hue_column].unique())),
                      data=group_nor, 
                      markers=True,
                      style = hue_column
                      )
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("Set1", len(group[hue_column].unique())),
                      data=group_bad, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'size':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.7, rot=-.0)[2:],
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'algo':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("Accent", len(group[hue_column].unique())),
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'th':
            continue
        elif hue_column == 'dim':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.3, rot=-.8)[2:],
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'dynamic':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("ocean", len(group[hue_column].unique())),
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'env':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.color_palette("cubehelix", len(group[hue_column].unique())),
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'env_dist_weight':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.7, rot=-.4)[2:],
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'info_weight_rate':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.7, rot=-.6)[2:],
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'round':
            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker="o",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.7, rot=-.0)[2:],
                      data=group, 
                      markers=True,
                      style = hue_column
                      )
        elif hue_column == 'sensor_size_rate':
            continue
        elif hue_column == 'mal_sensor_size_rate':
            continue
        elif hue_column == 'badmode':

            ax = sns.lineplot(x= x_column, 
                      y= y_column, 
                      marker=",",
                      hue = hue_column,
                      palette=sns.cubehelix_palette(len(group[hue_column].unique())+2, start=.5, rot=-.0)[2:],
                      data=group_bad, 
                      markers=True,
                      style = hue_column
                      )

        plt.xticks(np.arange(x_tick_start, x_tick_end, x_tick_dur))
        plt.title(hue_column + '\n' + x_column + '_' + title_name)
        plt.yticks(np.arange(0, 1.05, 0.2))
        plt.legend(fontsize=20)
        plt.xlabel(x_label, fontsize=32)
        plt.ylabel("Accuracy, $R$", fontsize=32)
        plt.tick_params(labelsize=32)
        plt.tight_layout()
        plt.savefig('../' + output_dir + title_name + '.png')