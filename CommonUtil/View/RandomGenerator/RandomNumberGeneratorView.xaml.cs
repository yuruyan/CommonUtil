using CommonUtil.Core;
using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class RandomNumberGeneratorView : Page, IGenerable<string> {
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
        public IEnumerable<string> Generate() {
            int minValue, maxValue;
            try {
                minValue = Convert.ToInt32(MinValue);
                maxValue = Convert.ToInt32(MaxValue);
            } catch (FormatException e) {
                CommonUITools.Widget.MessageBox.Error("不是合法数字！");
                Logger.Info(e);
                return Array.Empty<string>();
            } catch (OverflowException e) {
                CommonUITools.Widget.MessageBox.Error("数字过大或过小！");
                Logger.Info(e);
                return Array.Empty<string>();
            }
            if (minValue > maxValue) {
                CommonUITools.Widget.MessageBox.Error("最小值不能大于最大值！");
                return Array.Empty<string>();
            }
            return RandomGenerator
                .GenerateRandomNumber(minValue, maxValue, GenerateCount)
                .Select(n => n.ToString());
        }

    }
}
