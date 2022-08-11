using CommonUtil.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommonUtil.View {
    public partial class RandomGeneratorByRegexView : Page, IGenerable<string> {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty GenerateCountProperty = DependencyProperty.Register("GenerateCount", typeof(int), typeof(RandomGeneratorByRegexView), new PropertyMetadata(8));

        /// <summary>
        /// 生成个数
        /// </summary>
        public int GenerateCount {
            get { return (int)GetValue(GenerateCountProperty); }
            set { SetValue(GenerateCountProperty, value); }
        }

        private static RandomGeneratorByRegexView? _RandomNumberGeneratorView;

        public RandomGeneratorByRegexView() {
            //CountList = new();
            //for (int i = 1; i <= 100; i++) {
            //    CountList.Add(i);
            //}
            InitializeComponent();
            _RandomNumberGeneratorView = this;
        }

        public IEnumerable<string> Generate() {
            return Array.Empty<string>();
        }

        ///// <summary>
        ///// 生成
        ///// </summary>
        ///// <returns></returns>
        //public static int[] Generate() {
        //    return new int[] { };
        //    //if (_RandomNumberGeneratorView == null) {
        //    //    return Array.Empty<int>();
        //    //}
        //    //int minValue = 0, maxValue = int.MaxValue;
        //    //try {
        //    //    minValue = Convert.ToInt32(_RandomNumberGeneratorView.MinValue);
        //    //    maxValue = Convert.ToInt32(_RandomNumberGeneratorView.MaxValue);
        //    //} catch (FormatException e) {
        //    //    CommonUITools.Widget.MessageBox.Error("不是合法数字！");
        //    //    Logger.Info(e);
        //    //    return Array.Empty<int>();
        //    //} catch (OverflowException e) {
        //    //    CommonUITools.Widget.MessageBox.Error("数字过大或过小！");
        //    //    Logger.Info(e);
        //    //    return Array.Empty<int>();
        //    //}
        //    //if (minValue > maxValue) {
        //    //    CommonUITools.Widget.MessageBox.Error("最小值不能大于最大值！");
        //    //    return Array.Empty<int>();
        //    //}
        //    //return RandomGenerator.GenerateRandomNumber(minValue, maxValue, _RandomNumberGeneratorView.GenerateCount);
        //}
    }
}
