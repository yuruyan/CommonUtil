using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonTools.Utils;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class DataDigestTests {
    private static readonly IReadOnlyList<string> StringData = new List<string> {
        "",
        "Visual Studio",
        "(*´▽｀)ノノ",
        "\r\n\b_\a\f\t",
        "[TestMethod()]",
        "abcdefg",
        "private static readonly IReadOnlyList<string> StringDatas",
    };
    private static readonly IReadOnlyList<string> SourceFilenames = new List<string> {
        "resource/DataDigestTests/test1.txt",
        "resource/DataDigestTests/test2.lnk",
        "resource/DataDigestTests/test3.zip",
        "resource/DataDigestTests/test4.txt",
        "resource/DataDigestTests/test5.docx",
        "resource/DataDigestTests/test6.vsdx",
        "resource/DataDigestTests/test7.xlsx",
    };

    [TestMethod()]
    public void SHA1Digest() {
        var expected = new string[] {
            "da39a3ee5e6b4b0d3255bfef95601890afd80709",
            "91b163a97867c591a7ecc9961dd251208ca0ce01",
            "9b79627cceaae9044b8a10fd691fa96653117a3b",
            "0c87c68a8a165dcbcc25e7e9ed98a2476e5c1564",
            "7fa195b568e3b31f3a2f87be308b928ff0c3b92b",
            "2fb5e13419fc89246865e7a324f476ec624e8740",
            "642302ce504bd9dd02c54aae9d3734f3bd777056",
        };
        var actual = StringData.Select(DataDigest.SHA1Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA3Digest() {
        var expected = new string[] {
            "a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a",
            "1e969507c185814b0a505b28711b2dfb8084220fc66d92a1aec15e64a2a758ea",
            "da30caebfc58c3180bf2fa9dfc485934f4c7f33a752fa259199c75fe37413f58",
            "ffeae2e33dfb558d94c33a2b89f719325079a28f8a583d5af6c8e206408f796d",
            "854fb47b9f8c07b49f328b7219a83771f07e80a58e0324c44ad4643b2d3942cf",
            "7d55114476dfc6a2fbeaa10e221a8d0f32fc8f2efb69a6e878f4633366917a62",
            "9a8dc03a5838162d8cb286318fb960881b3f7ec84e51cf2c7bf5b2095a7ed7d2",
        };
        var actual = StringData.Select(DataDigest.SHA3Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA224Digest() {
        var expected = new string[] {
            "d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f",
            "f8dbbe5d2e956878d2cd96e487d9ddee7ecdd541d3399f43f28839e6",
            "a052c8373277f05cd29b72b7176796e23c115fef89b57ac389431b00",
            "6a2f6072fb8e52f74efdf142f4b5e798c70858ee9f669b64739f4b39",
            "77949a582a6c067eee1ac0d115cca75c7f31d57e83de88111ebdad08",
            "d1884e711701ad81abe0c77a3b0ea12e19ba9af64077286c72fc602d",
            "cddeffbd23ec7947f11fd8ffecd91a47c0d7d500fb92e6170e6213ac",
        };
        var actual = StringData.Select(DataDigest.SHA224Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA256Digest() {
        var expected = new string[] {
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "026bf2b64617f7c3f5945dcecf5ff629ad7dbbe7a5878662f34b61e17870a467",
            "565271305a4f4a43fd06d3beae9f3169cae5c0e6d7bf65d91cbdfff2ab9ee822",
            "b3f4e6b711b2ce3a448ea5674f8b9f21353a1b9eac9b53ce5a670547e25aa8e2",
            "14f4522a55e1c65d84b71a79aac7d46253aec1f5bd8cad17f67feadbf8d32a48",
            "7d1a54127b222502f5b79b5fb0803061152a44f92b37e23c6527baf665d4da9a",
            "57de270eedb9120372c90f2fb3d147ebaf22543be7c4eb5e33bed235e237c74a",
        };
        var actual = StringData.Select(DataDigest.SHA256Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA384Digest() {
        var expected = new string[] {
            "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b",
            "c0469d76d5d6a5789c01f6c051b0bcb3c664cc4af4bae81aea0f811ab2e51ad0bc09b19bb4b343097d6242e5f67228e6",
            "f0af3b8140aa082b7b39d8c335d6bd98a7772e0def03e78d8a2e07e66e7db0d17190f70fd1fa76c2acfa88c80d0d0f98",
            "72688a6ec18dd3c27bc4847dd871ec3cac51cfcbc5fa73e9b7933af8d6830cc948ef4243a14f3cb4614cd56b6b37abd1",
            "40dbb4fe376c725784cf8a3e001e757db0322450058f5a71076272c1886e81cce2af612433e941d85a964ee450c843ff",
            "9f11fc131123f844c1226f429b6a0a6af0525d9f40f056c7fc16cdf1b06bda08e302554417a59fa7dcf6247421959d22",
            "66d84446c0f9d832323d3bae7f742f95d9c42589fff24c5cfa2796d03bdd502697eb36b17aeff7c336860ee182b8b1a3",
        };
        var actual = StringData.Select(DataDigest.SHA384Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA512Digest() {
        var expected = new string[] {
            "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e",
            "26e934bc544d68cfc3d9de39772d66497b11eecf07138f454009066dec638882f376c5e7c145d8d70ea68d2622492165eb1f17704d5b6fad768b86c034287154",
            "c9140876546b5d7c03c02a5dfa58e35cd58ad25b7a3d5e2de196b3c606ac5ded80db8ab68916e311d1117dc3ed23ce3a5dc038e2a70708f33880a360783ac39c",
            "0a297ffa54f4cbac09ad22078363cb191d28cd0a5069823040cf287eeca007db28f4abbfaf4664f6b390f5ae584d1e0e10c524bf71119e969e3696b81cdaafe6",
            "f64e4b9c8bf807d6f77d29ecd6f7c471497dd994280366de73ff6c1c8a38680342dd6823a157b9f94450d83c9b4b80ce297481f3d06bf392afa5f041fab94be7",
            "d716a4188569b68ab1b6dfac178e570114cdf0ea3a1cc0e31486c3e41241bc6a76424e8c37ab26f096fc85ef9886c8cb634187f4fddff645fb099f1ff54c6b8c",
            "5d6d8c75fa8050745c9111bfd64856492b28b30d0883676797f2f869a3da72bd5239ddf45eb5d9777efd1c095550ae9b0341345c479582ddc76f497a977778da",
        };
        var actual = StringData.Select(DataDigest.SHA512Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD5Digest() {
        var expected = new string[] {
            "d41d8cd98f00b204e9800998ecf8427e",
            "f84a3e55d2a5134a7bcd191345471db1",
            "bd9c1a511c7906767fa97fe2c1df82dc",
            "eb58e52df63dfc4d32baa2d6f67f9a06",
            "dd8c2fe004cc8b5b93ed0efab538367e",
            "7ac66c0f148de9519b8bd264312c4d64",
            "132c904cd39a6a4fbb4dfa812a5a7dfb",
        };
        var actual = StringData.Select(DataDigest.MD5Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD4Digest() {
        var expected = new string[] {
            "31d6cfe0d16ae931b73c59d7e0c089c0",
            "a683f73801b4c8da1d75bed7f67adb4f",
            "d0daf73e451cc0bd47d5ef6a7bbd6c12",
            "9576ffb0bf9d54d56867dc1f4453a5a4",
            "c9f5a89d9e0cc1e6f19b1385bd8c3d82",
            "752f4adfe53d1da0241b5bc216d098fc",
            "6e782e917b20518e592c8f2552f9018a",
        };
        var actual = StringData.Select(DataDigest.MD4Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD2Digest() {
        var expected = new string[] {
            "8350e5a3e24c153df2275c9f80692773",
            "733f977f798595fd6ff0a2d9917d7fcc",
            "8e75eb866c89e64e321d268e62a08be5",
            "c3d5a61f4c4ac28a724362b48e91d1f1",
            "2aef63f029f759bbede648474a9009e9",
            "7d3dfb5d37e327866e680dd379fc0c38",
            "4ccc893d1b69264b660fb8b7cb581f20",
        };
        var actual = StringData.Select(DataDigest.MD2Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void WhirlpoolDigest() {
        var expected = new string[] {
            "19fa61d75522a4669b44e39c1d2e1726c530232130d407f89afee0964997f7a73e83be698b288febcf88e3e03c4f0757ea8964e59b63d93708b138cc42a66eb3",
            "6f0dcb8dcdf006a1b9fb2abc7a81da567530a76a3ac1218f784f4ed9694816eecf9b544b49a1eac4cc7d259eb228f18c004c7576b6d551531d0674a3b7a4fd49",
            "aa0df86fa4ea0bb536a92d2f27b7e1cc822f3e1895975af3d795753fd08d5607825030bca0bfefacc0f85506e607158c693f305bb2538b4b0a1b5fe060e36a6e",
            "872ccb95a617e89d6847e856d4ea9c6fb6418a44317eadb2a828c52b3367780c67c0d2677fbec076675d260c04c261f50af6a740ee5a7f753f6bb1081c4afe30",
            "3d0c77f695d84d25214c9052c5800352b33a12029e16136ddb3f85c49ee400d3b47fdc91618369f27920b534257900dd0ecd717dc6aa6908e72a2b7b051306d2",
            "08b388f68fd3eb51906ac3d3c699b8e9c3ac65d7ceb49d2e34f8a482cbc3082bc401cead90e85a97b8647c948bf35e448740b79659f3bee42145f0bd653d1f25",
            "f4e30fbe818b23018188596795cf49c6847ac4bc6d47c9c56043a9923146fe172d5b637d10f97e52e1512e1ee33e07fe988ecf996afdcd128f34327ad064fc91",
        };
        var actual = StringData.Select(DataDigest.WhirlpoolDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void TigerDigest() {
        var expected = new string[] {
            "3293ac630c13f0245f92bbb1766e16167a4e58492dde73f3",
            "459b2fbad535b62fde40696eaaf2b0f4e71da1476c1a4d00",
            "de8ea38f06b758c44bf355237ffd0f3ad595fe4e268d974a",
            "5d1f5ccaf6d72217fdecfafce9e2c5cd35aebe4cbc81c31a",
            "9eb380e932bc712d2b2d22f40f7c5849553afff2e2d78ea3",
            "39cfb8a0a2683fac91c828dc52c586d23b73711f63e02726",
            "6b4232af8fcb3b52f78c62eaed97e654b5d1fbc3982cc638",
        };
        var actual = StringData.Select(DataDigest.TigerDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SM3Digest() {
        var expected = new string[] {
            "1ab21d8355cfa17f8e61194831e81a8f22bec8c728fefb747ed035eb5082aa2b",
            "1f25829d7abdf1ad1c5b5493e2271d8dacf2de2c697e79eec4c232192ee164ac",
            "31339ca08193dbac2ddb90b3b220b67bc6eb8b28d181fca0751e5586f6f967b7",
            "c0d9ae5c08a659906d93453e316a6e70758feb018846ec8d9c299ae70510946b",
            "37a162d6af93580110bee869210896a231d70e49e880a853ebc62d4e49b2d033",
            "08b7ee8f741bfb63907fcd0029ae3fd6403e6927b50ed9f04665b22eab81e9b7",
            "9065f4223e74d36daf5d781d38669f7b8aacc4325ba286f3a99f97d9cd209a65",
        };
        var actual = StringData.Select(DataDigest.SM3Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Sha512tDigest() {
        var expected = new string[] {
            "c672b8d1ef56ed28ab87c3622c5114069bdd3ad7b8f9737498d0c01ecef0967a",
            "849bad600ac2624197510442aebdbe79e004ae3f73c3d8e22ad8eefc6dca62b9",
            "e0ee5d5a64132b4c7252903930f9c7e939aa23d209de83efc8ecc09d781a0a36",
            "1934976175ae4825e13239e4c916b9abdbcd684128989d005d26bf90ed592a1f",
            "2702541de2930898d9814529d2ed40187052cba1d0e0bdbff75d46bf28ddb4b9",
            "a8117f680bdceb5d1443617cbdae9255f6900075422326a972fdd2f65ba9bee3",
            "d27a1a8ef84a9318cbf99397a5228f3df59859ae69c9c16c559f65d3ea99a91e",
        };
        var actual = StringData.Select(s => DataDigest.Sha512tDigest(s, 256));
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SkeinDigest() {
        var expected = new string[] {
            "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621",
            "22bb376aaf6a27417566b5a01533bf38a0e1f5822e49cc3cede06a246cced1e0",
            "b3050f7a39c5157f7ca53c4110460c11fcac17f62173d87e12d87a730d17a077",
            "e5f792cff080fba1157af7b376559f4290edd62e112999cdf1c6212e125194ee",
            "c66175b324b95a0dbf23656b1b9d5c86ea1aa92d947b6f2a63ac2ed14e9b5b11",
            "200b07ccf2b002b712a19cf09d0a12a47e69cd8758afa27af7098f78b79b7ce1",
            "4fe4116ba2fc52aeabe0a3911dc5358ceeefc48382a6989c787fc371a17f4680",
        };
        var actual = StringData.Select(s => DataDigest.SkeinDigest(s, Model.SkeinDigestBlockSize.Skein512, 256));
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ShakeDigest() {
        var expected = new string[] {
            "7f9c2ba4e88f827d616045507605853ed73b8093f6efbc88eb1a6eacfa66ef26",
            "b2855906c645748200655b87dd19dd3e3df0ab04d26390f299ebfe0a8c6f5736",
            "35b3e0e00551badbd11b560a27caaedf2199238dd792156ab30785c2e08e4a25",
            "719e5fc24f30fd9b468514dfd6e2f9918c1a341248a9bda1f8c52b884c917044",
            "47710ffe5780e89fd36009e6422217936b1663dc9617f0067818ac503f5e1378",
            "7996db9262ea1e523aa160aec09ec5e9ca65a8319da136b88efd826123d4e39a",
            "53d9f04c71d1c23f26af966837311b2e77974b32d5b23000c23aad7e4e0a78f6",
        };
        var actual = StringData.Select(DataDigest.ShakeDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD128Digest() {
        var expected = new string[] {
            "cdf26213a150dc3ecb610f18f6b38b46",
            "4b63ce973bcf37598697efb6d4f2e4bb",
            "4879cf14ea1b91b8d890a6ff83b6156d",
            "f5b5bddd58a45f761958564365875706",
            "54aa11255272d95352dde25bed2a0b63",
            "37d1ddafec44cfae3dfcfa831b9ad3fd",
            "c0a05f835caf0156d3457c60d6f86f4e",
        };
        var actual = StringData.Select(DataDigest.RipeMD128Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD160Digest() {
        var expected = new string[] {
            "9c1185a5c5e9fc54612808977ee8f548b2258d31",
            "e567229ab5e4f143201db4bf4329b4432333209f",
            "8eedeeead959f6984e5e492da7e0292a4756ab53",
            "9cba4dbae8f682d1ee985e07c1e106c797e3a589",
            "80e371f942f7054e1a46e6603d13263166b1cea3",
            "874f9960c5d2b7a9b5fad383e1ba44719ebb743a",
            "730f6c372dc29a2c4de9ee16a028d720a182811d",
        };
        var actual = StringData.Select(DataDigest.RipeMD160Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD256Digest() {
        var expected = new string[] {
            "02ba4c4e5f8ecd1877fc52d64d30e37a2d9774fb1e5d026380ae0168e3c5522d",
            "c317aac8da3db4ce373faffcda6f992119acba4a891cc8568cd471fc475c7ee4",
            "685369ae15ab4a82e2ecb82cffa943ac6b5d56a19631010fa7710ea822054645",
            "d820664c0d37ccddacfb4c7330ada66cc21d5486fd4a17eb71690c16f7bfd2fe",
            "b6a71a5afd8a16d8760b52a1ada73de6aae298a73cff30266c24df554de9b7b4",
            "d1ba79b6f5853d181f10b18d127e628ae569e4c5e28a3a394ac7edea7e28872f",
            "414895f2b6992f38ce8770cb98504dc6d8a9e7fc7119361017fc99836a8665f7",
        };
        var actual = StringData.Select(DataDigest.RipeMD256Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD320Digest() {
        var expected = new string[] {
            "22d65d5661536cdc75c1fdf5c6de7b41b9f27325ebc61e8557177d705a0ec880151c3a32a00899b8",
            "0179f36019685c573084b4a172238fcb6ae6f528ab99d861abd5aeae87c71ca347f9b981887639d1",
            "6318c7535e5afb1817f12a2a24a4b54af0f69f067f683564aaf9b90d8e4a266068c91093e077b524",
            "95ce534d35745cf55396b15e1b00993b645d74a15f461df050f81563e109edb49cf893105853afa4",
            "17a19b19d1a2ed7e02b08f2ad8762a851c58158b6bd756a71d1b4e3f16c20543f3656dac7579363e",
            "43d652e9b04af988497088eeccbbb7a858b4468acd20b94b2a1ecb7194b48c4c22742652e11fa2f7",
            "406bd9cd62b3041ef722a5094b153188eeba06dbbabb81927eb8e543f66a2e006de47c647c3044b3",
        };
        var actual = StringData.Select(DataDigest.RipeMD320Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void KeccakDigest() {
        var expected = new string[] {
            "6753e3380c09e385d0339eb6b050a68f66cfd60a73476e6fd6adeb72f5edd7c6f04a5d01",
            "dc712aa212563d505f53dcbbc99a6bb9ed2d8f4bfca72b20f85bd94813a164bee74cb8b1",
            "d6226f2bd770248881da4c2b0e9f158e4514bc273c37bc1f9417435e380a403e7d46a3de",
            "5df2f96a8caa250770ebc46150d65a6e2c107739a75e866218185737237dd7f12542e128",
            "e74f645cfb31e25c0a5b706834164f4d50110ab25486a776f47cc623bc9d90a430e0da59",
            "3f0dac2a968f0ec92c3efef13c0701899cf356f8bd241fb3210b5d3e1937b288411fbf87",
            "41f3461751e56ff7e435a56264abcca4275fc5ac297a2c359a9fb7ba8d92b8c04e43b21c",
        };
        var actual = StringData.Select(DataDigest.KeccakDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411Digest() {
        var expected = new string[] {
            "981e5f3ca30c841487830f84fb433e13ac1101569b9c13584ac483234cd656c0",
            "ca9c85ff7fc5716494e8641ff71dd84bc357dc1e64544879addbcf100ad05b67",
            "823b58c2f82f856d9af35c1c6b878e60c43ba70d40c27658e4fc95a42871f076",
            "68f146d913fa7340010f1f8d510d242507af2bc244424dcc9ed5556c1fae6941",
            "8d7604f8dea037de3c99e14c417696ca02cc9659a957d36daa65ae72b3b7ef6f",
            "e7824b1446b958f302800d8d52f39a5768319a8c58cd9a4ba2b4a8bdbf033897",
            "53cb08d7311653a332447807367cecf1f562b78feaf416c8074a403882ff24ec",
        };
        var actual = StringData.Select(DataDigest.Gost3411Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411_2012_256Digest() {
        var expected = new string[] {
            "3f539a213e97c802cc229d474c6aa32a825a360b2a933a949fd925208d9ce1bb",
            "662ac06b045fbd49d9dadae0569e964bacaecec4a1aa66404f71e6c807dc7504",
            "e37026c97a7a1e58ddd7c0527cf8c173efe0f964a1c7249d8fad217fd995eb80",
            "845268b30e68965f2931524f7f4ffa1ead951a044064daa08763846312ca9b79",
            "343311b03c0a1c9fbebebdf921949effd7ed36b59db9d1e0f8a0cd1d2fdcb919",
            "5ad83263aee66b0620b8b6abe8432b6283360a3d60bc287b7e2bf41b19b318cf",
            "83ad8f8046c71746298d3e69cf44668d559af5e6b5644dac4a8ac098850761bf",
        };
        var actual = StringData.Select(DataDigest.Gost3411_2012_256Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411_2012_512Digest() {
        var expected = new string[] {
            "8e945da209aa869f0455928529bcae4679e9873ab707b55315f56ceb98bef0a7362f715528356ee83cda5f2aac4c6ad2ba3a715c1bcd81cb8e9f90bf4c1c1a8a",
            "236267ae03d129cb66bb33ca7001cc409acf291e04bdc7f6062674218ecae9de96c9c96a899438f33199ec9d6af51f30c454bc761ac64bf9523a43354bf6d6d6",
            "8d168acd15f743a9bbcb9be76353e803ddc396d8fc425f6292f85326cd58b4303262b9b34a15ee6740f82fe30182f613f2f096ac339467390384cbacdd2f3b7b",
            "256e29ac173da4a64a2eee4b575fde9c7a3f4cbde5f023c7c82071e9a4c93a146d5fe7e6d6c49905fe9e53114d15bf5d9325f89f218c150f360eea3d7c52fbb5",
            "9d1c4208e5cc1579fef39ce7bd23473cfa8a6df4803555541e15a49f3d24fe62a448cfa9ce368895b038a46201697f4f75154947a25ee1d0e661a484f85d27e9",
            "19d6b632fbb5e9c9d5094ce1ed11816fabb66ce6d7d3437b669942e2bbdf238aaa89c24341c1d17a3c1a2c14f77a2613a6ef27ce5f3326b3343fc26bbd884272",
            "6466e1151c869653d05dcc46d85584563db57ac6c83a39102978c253cbf2253c204c0a23f184c05d0f64116ab957da80ada9aaf0352305ea08fd8d62adc0dcd7",
        };
        var actual = StringData.Select(DataDigest.Gost3411_2012_512Digest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Blake2bDigest() {
        var expected = new string[] {
            "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce",
            "13ce9d894bb129ee8f52820929d12921c772d22fe639aee8a159206b0a00164e501fc49d0cee2d347c3584cdad6e55a9a500be890f28a631b43ab76249ab761b",
            "afe29b53a340b940311bc471802eaab23e8f3a76cebe1c229560c4f565d8c89f26960eae574b57f89943d6522ad89ffa34de1ba9b99d170befeb2a886c2216f2",
            "72591be93250e89884fa4b3125e3a1a1f8911a50038ffdad6d108c79d4d3e6d028ef82fd91afabdc047dfb22b8cd0c0c035a0ca29c8b4ecc250fb5c819c1284d",
            "34d0b41497acac989af0ff4e646e18d4f91309f19b4d2f7f0dd48174a1c199cf4079c1cabec24c7e464d92318581d41ef911d96ef17caa89e9fc0c8113d3bd50",
            "81e659403d5bfd8aa7f8ef2beab97c4b866a27b0d1079d1d97e6915f65d6e947f4b2efea807c4568fa6e201dd79c4a82d6988c71b7cc4a9673575cb3a1cb2202",
            "7c6cb06988d481a53a501bd094978b8a597122fd6ed7680854d3d1ee7066927e4fc275aaf50bed2af37dbd313b2c2c4225361f33becadb4cba6faf9cac263aa6",
        };
        var actual = StringData.Select(DataDigest.Blake2bDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Blake2sDigest() {
        var expected = new string[] {
            "69217a3079908094e11121d042354a7c1f55b6482ca1a51e1b250dfd1ed0eef9",
            "02c3d0f1d2baf7756db15c921d6a0efe16fd7b5b9c8a4ffdceb5c46cc87fbdc2",
            "158c25af450e34339039ba0df54f658ac8652de591d2891d96f911a566fbe123",
            "56677aa5d3704bea2af2d298c78091e23c8aa612a84bf84ddcd0dde103686480",
            "b0520ac8f86eed8798e3b542e27739ae37b5dd07db83abf0f1ac9db373f5be65",
            "4468a2b5329224c54c243d4fae24cdf050a27a563480f4af5fedd4446d8c4a04",
            "ba63e3052406c1559f89e39c67d0f8eb4d8324fd3cbc7e3128809c3646f32360",
        };
        var actual = StringData.Select(DataDigest.Blake2sDigest);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA1Digest_FileStream() {
        var expected = new string[] {
            "da39a3ee5e6b4b0d3255bfef95601890afd80709",
            "4dd514fd3a459993f75af420aad2edbef613c85c",
            "96574cb5cdefa1e3db970ad6bfc457052db0646e",
            "77b5ed9ad061cd241a401b7547b50921df627bdc",
            "dbf7940c4ee7fbfa634874e2422d9e95024d8ed7",
            "a4daadfa33ed16c0892e26b207a971bddfdf4578",
            "7df182b3a931dee3b05068a321f03eed05501231",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA1Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA3Digest_FileStream() {
        var expected = new string[] {
            "a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a",
            "90cac30b33bade26de978f88d31a56b42283360f5e536499d5f2595e6ac38a19",
            "d82d049542a37336c35403caa2bb88e0e9b1ddf29b5421a19138ae77202d4413",
            "a1e081cdca37fd638b7e3008efa93efd639584f91bbf70c4044ddf9d66ed47f3",
            "830e9eb436ded4560ec8526aec499a6527362db77de7ad793e8494d6b8e477fa",
            "a5e77ac071e43c36d95a1aad8e6ab7b17ee84834409088f1386a0769d62d1550",
            "6d55f933c9176f2314f5af9b926775241ba6cccb69d579442d5d2360caa34d21",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA3Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA224Digest_FileStream() {
        var expected = new string[] {
            "d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f",
            "e80582974446af57023d03a9b53e24047371b894777049cbbc568157",
            "b9c86dd9e26dafc1494b4e082c34f65a1f8fb3407070d2f3e1f493e0",
            "3b7890909c1a9039de52eff619b77ac08ec15ed0dc969ecb47075688",
            "e57ef1afd88a8bc21a55e3660a893b8fea6f1318e2513c1cbeba11e5",
            "64bea4986d8d0a5f504985cda621f7caf3f602e703799051447598ec",
            "0850e3f159280c411b1727ec5ba8c3cbef5b8322518fc91d29fb3b67",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA224Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA256Digest_FileStream() {
        var expected = new string[] {
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            "a48a2af237053772c1c625bfdb93cf3185c671fa96c201fe50f40cdbd100e83f",
            "abd69128ea94c14d1495b95cf871debbe7da163d5a7f6342e7d691df7b49ad0d",
            "44dc8dd69f049c3e0b9ee02f2394a20e7ff2294416bd5f93ab09abeb2d615b21",
            "9c49951b519d49a6a565c14196cd0dd6beb43a9586fdee38fb98c8222ced3d10",
            "b0b0416a915af44baa3fec5a5ad70403f66be44462d47c2883ea1372cbe74899",
            "05304d1696e98590403de459e338d71d46bff0194894a11dc9c8d19c5061be4a",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA256Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA384Digest_FileStream() {
        var expected = new string[] {
            "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b",
            "42f08ebd9178100a1311cdf93000aa5ca369f7e15f657f206cc58e07475530bb9edb4198fdc8f947cf6ba0b943dfdb71",
            "71139272201f6240b544be488675855a604adfe140778a0405633d3dc1336df2e2b998a5d05ad7594720eb2fe42c0071",
            "fb62f96dac306d68f8c2f09ca19253fa3d687c9d17a687a68aa21b81ccbe3ee0d3ccf3bb9dc1fcbbb4e69b78072e8960",
            "34c6b243455fe47ec5660d657d9c200754aaa83ba79dd0c02cfdb0b4b51ee2d2715c128d99681a1875193692e6668b75",
            "a6408b5a91635362dd03c2617b77aa4f39b94c609ddb3d3896730832213de7a08721c3136693c085ec992cbd24e5266c",
            "8bba226d7f90dd5f93b1badfff025913c951c981725b3a0d80ca25040283c6e90f12be458692d9ef7b1dc1edea5d850d",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA384Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SHA512Digest_FileStream() {
        var expected = new string[] {
            "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e",
            "eb803f55793e5635c564fcab4998f7bec26bc2c1181b937a910585d8e94e620ccdf449541739d7b482a060c9d22935d282f8967d97d7ef3f4943f4ebc9a44fc5",
            "21f2bd74dded1ef5efb97d1b93bb41797541961ea54b6f185224b73cf0495d0875072b104ea97170dda7f86f3e6059ffb85aaf25f126da76de0214ee5ceb57f7",
            "bf1777ea7f957076e554b194535d6fcff0fb0882dece540e50f25c8f650a5abb428f4031a66c67f9e220f4a0ac209e2f259c9d622805ca3c4cc7c76879ad017e",
            "58c247dc5236f4c60c08b74369ee4623391abdec26ba531d355c75b53d7f30f3599f6d4cf00ef516ce5c9c7cfc3d6d32ef0e4f3c7205deccbb1127e793618ea6",
            "ea1bc5591fedb9620033b4ca1f36251dce4a49c6af2bb4fe1b514e5168b188859b2d0b35ecc3b1f6f6430697f8d70d1582f4a22b651dbd119de9ec5b3c056668",
            "49671de80ec9ee4a026f3305b13d979709f6090a91225bfb9ece4c39e3ce59725bc19c321406ee9750198bccbe5b9310f57ca986f542811770759ca9426df649",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SHA512Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD5Digest_FileStream() {
        var expected = new string[] {
            "d41d8cd98f00b204e9800998ecf8427e",
            "02d883d0d669753d359a351ef0946244",
            "6112f835b930d19a631d8dba43899c1c",
            "9415cc4b27654954313c69ea093a774c",
            "ec04dd11a74c25bd617187c967aa336a",
            "66f9ce93e4db0732f2673fcd5f68afe2",
            "7294412d91e4e7178cd511346257e35a",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.MD5Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD4Digest_FileStream() {
        var expected = new string[] {
            "31d6cfe0d16ae931b73c59d7e0c089c0",
            "aa694d6b1ca0ed23b49b2a220e866378",
            "9b72b04dce182f694111ef7b6269a535",
            "7203b8be0af2bef14acd9e00dc9539e7",
            "6ed3582cfb0bd02bde85fe71bef06694",
            "2302ce6d5b9a7a8ddfe3309c2fbf91ed",
            "a4b1eaedeead1a35d58fba4154e4f05a",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.MD4Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void MD2Digest_FileStream() {
        var expected = new string[] {
            "8350e5a3e24c153df2275c9f80692773",
            "eb53c90bf76c6fb46b330a0ca200b50a",
            "8fd3f3105b8c6f65fcc33889a3f61b3d",
            "887e3344640be27eb61e5568816770ea",
            "092f3974104e75ca806df0194edaeec6",
            "109a1b6d1c10ef5a59094d9bcc095f1d",
            "d7dfa38635534b196a538e4756823042",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.MD2Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void WhirlpoolDigest_FileStream() {
        var expected = new string[] {
            "19fa61d75522a4669b44e39c1d2e1726c530232130d407f89afee0964997f7a73e83be698b288febcf88e3e03c4f0757ea8964e59b63d93708b138cc42a66eb3",
            "947e8ef7069271e2e6e58eaf8e0b9e0d9ffd5cfc35c8e3a15a273c49bcfd972e2d20c252f3c45fc1c7421174be16f318078f427b9cc767d9a1a4a8020deeb26b",
            "fb7d92722dfdfdf6ce9adf88ce0119a2f60e62a9dd34ccfa671854ff1cba1abd5702f8f77331183af72ea8b53296de9a166f29a648796dd88e717ed106743e86",
            "cca6857ccf2d74df51f6b43d7c32e7c258f44746aed1ad2becff561cd182d06d3070b1aaab00ff1a558ce8d78016cb707a44413bcf55580782ca30a7f2585140",
            "ba0bcc0b44ada98fe5232caf692dcc6af4a5216960cdb9eb18730c5f046f475f94de90d61af42f0858dfb20a656f2c920c921576e2aed12c6a92b23cc92c20a5",
            "96147ca0ee68c3be7d78d398c69af0918371ac7ee828e78437eeeaf0b950756b3a16f593ffd6388914f96bc4656d326cc16e4231e1e8f70730f142938134f1f3",
            "4fb366f8188a2b220edabf29b5a978c4e85215f328e795e930812a676044a00244126ce799c1057f4d9704506f65648a092a3693c9c80f234bd3631b2c29e002",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.WhirlpoolDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void TigerDigest_FileStream() {
        var expected = new string[] {
            "3293ac630c13f0245f92bbb1766e16167a4e58492dde73f3",
            "13fa8578c9d4412b960e780604b55136275e9f665b27c143",
            "505bec4844577d93fe290bc29707ef57a5085de7392d491e",
            "5927a3f6a94927c20380835bf107df91f718d8f00016aa3c",
            "800aa238105900ae47a28ac12446fd3e00dfedf0bafbf37d",
            "838f6cd689dadfcec265549ab47f4ed80aea91c83dadcff2",
            "3baa1690a2b1acd5fcd86d4d8f6f7cd8f4a6f19c1899d42c",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.TigerDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SM3Digest_FileStream() {
        var expected = new string[] {
            "1ab21d8355cfa17f8e61194831e81a8f22bec8c728fefb747ed035eb5082aa2b",
            "108178b0112d9bf1f4ba4bfe15b5f1d2e00e01e801a7d0511f40fd1831fd3472",
            "89e545f6879b2895bd79dd4d34e8c907705c19216a39a97bef68354a513dedfe",
            "8b803d7816c56216b1571e0e6370663de84570135747a9dda1c12dd5f8504ab0",
            "d9aeb9a4bca08ba20a4309b5d54f9190b5bfefad1b1410e637a6e6661ac2f3e0",
            "a6919f9a15f0c5e30b14eccc50b8c70b508dbbd1b9e5d06ee5caea438efd1646",
            "030139775371b6202e267213cdf7045e54c3dd5140b384cbba90104fba6bc47f",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SM3Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ShakeDigest_FileStream() {
        var expected = new string[] {
            "7f9c2ba4e88f827d616045507605853ed73b8093f6efbc88eb1a6eacfa66ef26",
            "bcdb518e0dbfd33e0faf5539ce4f1d1e05c229f03c379d59981324622159b9d4",
            "f35abf8eb845160ce03faf3170d7b6442c8996e333f34f490ffae8fffd596c1a",
            "9a6d7525159f06af274630ea785e8c889ea33eef1e7d30f6910134275a837bfb",
            "3211487d339479b549407b24d659a88b8c567fba9fc2888213ef997fda28e1f2",
            "2162e9cc311d4eda6e180848df7b76cc10415245a0aad81269a2cf7a50174328",
            "de0088c6dcb7b2cd356a46af44db847736b954819174bcae30f3c863dc7b6cdd",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.ShakeDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD128Digest_FileStream() {
        var expected = new string[] {
            "cdf26213a150dc3ecb610f18f6b38b46",
            "cb1ad52b93f21da5c53896555e72656b",
            "f8a02c385522c89bbed46662b6c5f28e",
            "1a8ea173e73eccc1babd7c6aed0ce31a",
            "506cf639c66f9d621be22b3e3001dfd7",
            "098b81401e82a1d9f87aa9f8b73aa53f",
            "d3c63efc72439f92224dea779902c2b2",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.RipeMD128Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD160Digest_FileStream() {
        var expected = new string[] {
            "9c1185a5c5e9fc54612808977ee8f548b2258d31",
            "3056d25efeaf67d585e0df89c57cec88089c89f5",
            "b24ea92d5c119eeb6cf226d3c9a2c5df67f2a59a",
            "33d3fc08d4ec8f3a032db9fb5c5ca3bd26f11afb",
            "e7c92e4b00c236b0ca6aa2a21514656451da6200",
            "36f6ab2c9c0cc89f8dea48261e10ab02eb6d5f0d",
            "8ccee9e346d01a449e1006f6e893952842236116",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.RipeMD160Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD256Digest_FileStream() {
        var expected = new string[] {
            "02ba4c4e5f8ecd1877fc52d64d30e37a2d9774fb1e5d026380ae0168e3c5522d",
            "db87b95fbfcbf1ef7352baf4a3d6be2c0f6d12208ce57fd94d90fd542fc16097",
            "90103565f778e0d878ee635ea8e5d4d514af97608e4ed90a8965786d3bb635dc",
            "d44ef0b88f0b16d260a79d2998b7ebdd30fc2791f639c32bd8fe21e107271e5c",
            "df06d72be59944aa03448e56a29fd0cc701798395aa0035e39a4ddbc73f6d0de",
            "af64fbbd31ac47440c3994a8176aaa692e2c67360aa4558381eae36f530c7ce7",
            "975628de12d4929c99ac25bbaaa04949e796064860f80c956d67142673809558",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.RipeMD256Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RipeMD320Digest_FileStream() {
        var expected = new string[] {
            "22d65d5661536cdc75c1fdf5c6de7b41b9f27325ebc61e8557177d705a0ec880151c3a32a00899b8",
            "d5714d7c5eaa986876a2a7f275359b407ef8fede649b12233002e3225f9c9205224ad1938dc74046",
            "e18282c066e3cd57fe80879b50abd39a21470be0811bc311425d42363fcfeb3eaee4156f4afc2706",
            "28feb7167d6af717f56703df580b37f8d810621b638c9a30dcab35aeb3213ad123c90098eb9b5f76",
            "b0efafe66e5a492b5482467bbf1d05233a27311e7ccadf298fd5ac1121235478fee11fca94d9aa2d",
            "d910d9b44f7a1cdc3dc5a2ceec65a3911ce176635299afe84ade7c3a7c1b922ca06d9585a73882cc",
            "f61074f66eaf8f15b05d0c93118493b94580decb1669e457a8a77d0b316273122f2664912029baf7",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.RipeMD320Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void KeccakDigest_FileStream() {
        var expected = new string[] {
            "6753e3380c09e385d0339eb6b050a68f66cfd60a73476e6fd6adeb72f5edd7c6f04a5d01",
            "1a88e496498780a3d4f53893cdead1a18cfe669dc411c55eb1a3797b27b859a3bd45d27f",
            "19ea73a5b8ae322440bb7756d1928250405b424e81e065ed8d760271c9b97d9c74702013",
            "e3ca1361d7c5baf3a88942346683bddcf78e1fd5d7128063897734a0dafe65ec3a785554",
            "6f113a29a06e7045877b8980ff996d09f0685b260baf15a4692768b6a8dd3d31e3c8b2d5",
            "651a231cec0f5dc9bb1e9be6ba3e6dfb565ff21610fa3bc9a0739e17c8099e94d79c86d1",
            "ece61ef70a22287a64ee03c5bcefaa68f871151ab7b835bc71dd91a706dd1ebc21dae79f",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.KeccakDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411Digest_FileStream() {
        var expected = new string[] {
            "981e5f3ca30c841487830f84fb433e13ac1101569b9c13584ac483234cd656c0",
            "5b69bf83c7ae191b4613dd5d77be2b80cde293cecb4f4867623fab2cb0f92936",
            "dcb5ddee08133c99d809f59a81688e8de128192cbfe914124c3791d7509b066f",
            "598dca07c529a9ddcba149acb8f4f6b2e210120a3ace4697782d52bf91fd8c9b",
            "ee416179c70af6521d9918fb8fc43ecff2a3cbd223bcac6aa922b8e94f9578e1",
            "f7f7cd3efcdb263973f2dd302d5f6c976ebf83d1089975c63e15ee70b6f7461f",
            "314f2ae5b180f4ea24579aba51453832633f60e33ea74d2c555ac9b1eafd5f87",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Gost3411Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411_2012_256Digest_FileStream() {
        var expected = new string[] {
            "3f539a213e97c802cc229d474c6aa32a825a360b2a933a949fd925208d9ce1bb",
            "4863cf8cc52cda4ea05e53d8bb26ff441106807a9e94bcfc4392d72b126b2d98",
            "718948ad1c4958fc4cbcd157268494ff820d8e875c1c5c58ca3b628a5c765911",
            "3ad922e5ce7908e9d0bf1623010f6e47c5e421ce0553ae400c861fa1d4511f7b",
            "ca14b83a7d3e613db6e3ee904f97871130e84a154a0ae93f34057d30f42c544d",
            "38894fc55ce3cf01f8164e0e4f4dbc4fdd77117124bb66501016d198adc9a3bb",
            "079073c0f1173ad9af1092ed18b279a9d468d61792a7739b0f5921b1aa80b84c",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Gost3411_2012_256Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Gost3411_2012_512Digest_FileStream() {
        var expected = new string[] {
            "8e945da209aa869f0455928529bcae4679e9873ab707b55315f56ceb98bef0a7362f715528356ee83cda5f2aac4c6ad2ba3a715c1bcd81cb8e9f90bf4c1c1a8a",
            "faaf8a43a79f5dd5904c4c8b95b1c272e3e5076bb4fd3419a1ad095f921d9b1fd7bfedac4428dee1ab8d58b1a120590dcb2954791740afb5dc0986f424741522",
            "c20cad14a0eb219c5667212356ff04cd749f006fb62ce70014e1624a9241b926d56a6b218cd86789edfc2e377639344959128837e9f1804e35e1e3197a8ab944",
            "774251d083661e6c9e017a4633d6648e175efb5610be0c26b6590e0c18cc899890042cad0dede7f5389d7e9949b261dab5203fcd1c2d41f842caa4dcbe1993f0",
            "a37f75f8873bad56ab8d7503712431d7300aa1b540fc28aad08e20df9e097a235a7b61dc93dc08766999aea140467f3c43bd2217f866b80859815f7cec816c3f",
            "0504a7475c9fdedea0df5a4a0b51b3ee3af02cdd9242165be22ee10884d553292ea827e42b79c2f532d069312f8679278cd45bdc8d203470a09836e0d49733e4",
            "02d3183fb4b64bca1d04b7145d3ffa8a63eef9951252e35b18c690e47ca7adb71d3468f84eced4189400ce7a91d099baf652dede59cf632a66622e31680f7e99",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Gost3411_2012_512Digest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Blake2bDigest_FileStream() {
        var expected = new string[] {
            "786a02f742015903c6c6fd852552d272912f4740e15847618a86e217f71f5419d25e1031afee585313896444934eb04b903a685b1448b755d56f701afe9be2ce",
            "b68e052154f827d96fd5db18b16bebad60a16d312250fd34535ff61f5c3f5143c53bb90f2bb76206f2aad144be6295ab94f937604f7a24dc47f1b815ddb9f072",
            "0ccc931d422a4692c4c6348dff979b8d1d804368a18acf36f197ab9fd5fa5294b1bd90a0e3ee0541dbbd8f23d7aef43635137eaf1eadde614bf02a92da693633",
            "0588c53ef719e46ac24a81f7df6471d5a547a31aae96c1a147c86f466ec8289ced352afd7803d850976519e757204f3aa7af8353e8df6aa985dfe5ecd67517a3",
            "ee7b504374d50873f6f83f16ed9fcb1fc9e0302859f78d63e779a1a875053a2b09cf18042170875392eff57f3a13ead18e40d92516cc7831cd5b51bbe593bea9",
            "c3a8ee29139b693c47e4baf789a03c60a02f9b78ac85c06157948294ebed92d11962bc4d5c06fff19417a15da4484a056fcfd75d59eca2acdea46064e38a2cf6",
            "3bcbe7ed6ded04b30dfa680b7f65d7f377a8fe17e519400656644ea5c581cd549d44826e8831dfe4375ed6c863045cb9cfcf6ed9b630bbfd3d1b9e23401481eb",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Blake2bDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Blake2sDigest_FileStream() {
        var expected = new string[] {
            "69217a3079908094e11121d042354a7c1f55b6482ca1a51e1b250dfd1ed0eef9",
            "6e484e097d5795b72a16b3dc447cc2e0fc4ed59c936cff25c36be56cd1050f6b",
            "e9b53a3c3451f0cdbe74fd88b6e6954f4529525d1fd25e0cada548ead84f0011",
            "02f1fccd32d49ed55101aeb6ce59e41eeec9a1d3b6e18b50e2398ef3f9d84750",
            "5b72e1573fc6eba6bd35857710eace7d0d4673d5ea8207c77c9ffed9f6d6c990",
            "e7a7c62633ddda76cf83f5fb28a8de12fb59116532d61da2b9ae777cc39a65ba",
            "1d33c4013173ab3f3a6d0549f4433a6f93e68927c7a88db604127bcf0a016bad",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Blake2sDigest(s));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void Sha512tDigest_FileStream() {
        var expected = new string[] {
            "c672b8d1ef56ed28ab87c3622c5114069bdd3ad7b8f9737498d0c01ecef0967a",
            "c20d8305be8b52fa217fdee6a2d60e2e1430d16075598ca99e3fecff6a4c0702",
            "4a2419a17de4949140fda6fa87b7fff3f436be3245a1cd8208ed1e915f2e68fd",
            "9f9f186a9d77e2a321cda2fcfec1810abdefb3fdf9bff21b110aae809e04f4a1",
            "ae51d5d46e87d15e04026bf4895d9c80195c50ee0b1651f402cba58eb66283e3",
            "818526978860caef57ee79f388cbf80acfbf9912ffa91b198e4bcdfa7a9b18c0",
            "d99d5aa87c6862ada10eaf8a4b9a48e4caf3cced91e74a4061e0b18c55a06c8d",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.Sha512tDigest(s, 256));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void SkeinDigest_FileStream() {
        var expected = new string[] {
            "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621",
            "32de7d5eed332151d6d08b560dfdffbba35bcfdb436a9d60e83afaa333621618",
            "963ae0ac74b2cf5d9946900a9631edfd75dee7095a0ae7823612c8dd2642d214",
            "bdd83c9bbff2c3e2319b2a2769854ab7609b4d68edc01c8226e18d01d94af4c9",
            "fde5d312bcde413c89a02aea547d9a61b2bc04b22d107cf01f88802ef6408624",
            "536696a5daa0a2693b46a90d450b1c61d771d0cf4487d318f90d50168bbca344",
            "e5480dccf01251f8f64d904f2d86a8fc65f70649e028714d571b0449385a564e",
        };
        var streams = SourceFilenames.Select(File.OpenRead);
        var actual = streams.Select(s => DataDigest.SkeinDigest(s, Model.SkeinDigestBlockSize.Skein512, 256));
        streams.ForEach(s => s.Dispose());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }
}