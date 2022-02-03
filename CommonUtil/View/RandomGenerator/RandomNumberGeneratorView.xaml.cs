using CommonUtil.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class RandomNumberGeneratorView : Page {
        /// <summary>
        /// 数字列表
        /// </summary>
        public List<int> CountList {
            get { return (List<int>)GetValue(CountListProperty); }
            set { SetValue(CountListProperty, value); }
        }
        public static readonly DependencyProperty CountListProperty = DependencyProperty.Register("CountList", typeof(List<int>), typeof(RandomNumberGeneratorView), new PropertyMetadata());

        public RandomNumberGeneratorView() {
            CountList = new();
            for (int i = 1; i <= 100; i++) {
                CountList.Add(i);
            }
            InitializeComponent();
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public static int[] Generate() {
            return RandomGenerator.GenerateRandomNumber(0, 100000, 10);
        }
    }
}
