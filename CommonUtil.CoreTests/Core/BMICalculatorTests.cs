using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class BMICalculatorTests {
    [TestMethod()]
    public void GetBMI() {
        Assert.AreEqual("19.0", string.Format("{0:F1}", BMICalculator.GetBMI(1.7, 55)));
        Assert.AreEqual("55.0", string.Format("{0:F1}", BMICalculator.GetBMI(1.0, 55)));
        Assert.AreEqual("18.5", string.Format("{0:F1}", BMICalculator.GetBMI(1.8, 60)));
        Assert.AreEqual("23.1", string.Format("{0:F1}", BMICalculator.GetBMI(1.8, 75)));
        Assert.AreEqual("22.4", string.Format("{0:F1}", BMICalculator.GetBMI(1.95, 85)));
        Assert.AreEqual("20.0", string.Format("{0:F1}", BMICalculator.GetBMI(0.5, 5)));
        Assert.AreEqual("25.0", string.Format("{0:F1}", BMICalculator.GetBMI(2.0, 100)));
    }
}