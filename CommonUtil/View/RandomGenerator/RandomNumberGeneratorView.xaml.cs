using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class RandomNumberGeneratorView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty CountListProperty = DependencyProperty.Register("CountList", typeof(List<int>), typeof(RandomNumberGeneratorView), new PropertyMetadata());
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(string), typeof(RandomNumberGeneratorView), new PropertyMetadata("0"));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(string), typeof(RandomNumberGeneratorView), new PropertyMetadata("100"));
        public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(int), typeof(RandomNumberGeneratorView), new PropertyMetadata(8));

        /// <summary>
        /// 数字列表
        /// </summary>
        public List<int> CountList {
            get { return (List<int>)GetValue(CountListProperty); }
            set { SetValue(CountListProperty, value); }
        }
        /// <summary>
        /// 最小值
        /// </summary>
        public string MinValue {
            get { return (string)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public string MaxValue {
            get { return (string)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        /// <summary>
        /// 生成个数
        /// </summary>
        public int GenerateCount {
            get { return (int)GetValue(GenerateCountProperty); }
            set { SetValue(GenerateCountProperty, value); }
        }

        private static RandomNumberGeneratorView _RandomNumberGeneratorView;

        public RandomNumberGeneratorView() {
            CountList = new();
            for (int i = 1; i <= 100; i++) {
                CountList.Add(i);
            }
            InitializeComponent();
            _RandomNumberGeneratorView = this;
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public static int[] Generate() {
            if (_RandomNumberGeneratorView == null) {
                return Array.Empty<int>();
            }
            int minValue = 0, maxValue = int.MaxValue;
            try {
                minValue = Convert.ToInt32(_RandomNumberGeneratorView.MinValue);
                maxValue = Convert.ToInt32(_RandomNumberGeneratorView.MaxValue);
            } catch (FormatException e) {
                Widget.MessageBox.Error("不是合法数字！");
                Logger.Info(e);
                return Array.Empty<int>();
            } catch (OverflowException e) {
                Widget.MessageBox.Error("数字过大或过小！");
                Logger.Info(e);
                return Array.Empty<int>();
            }
            if (minValue > maxValue) {
                Widget.MessageBox.Error("最小值不能大于最大值！");
                return Array.Empty<int>();
            }
            return RandomGenerator.GenerateRandomNumber(minValue, maxValue, _RandomNumberGeneratorView.GenerateCount);
        }
    }
}
