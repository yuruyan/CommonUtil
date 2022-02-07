namespace CommonUtil.Core;

public class BMICalculator {
    /// <summary>
    /// 计算 BMI
    /// </summary>
    /// <param name="height">身高 (m)</param>
    /// <param name="weight">体重 (kg)</param>
    /// <returns></returns>
    public static double GetBMI(double height, double weight) {
        return weight / (height * height);
    }
}
