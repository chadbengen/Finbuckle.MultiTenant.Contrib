using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.Extensions.Configuration.Memory;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Finbuckle.MultiTenant.Contrib.Strategies.Test
{
    /// <summary>
    /// Tests different ways of binding and injecting configuration settings. 
    /// </summary>
    public class DITest
    {
        internal class ConfigurationTest1
        {
            public string Name1 { get; set; }
            public string Name2 { get; set; }
        }
        internal class ConfigurationTest2
        {
            public Dictionary<string, string> DITest { get; set; }
        }
        internal interface ITestConfiguration
        {
            string Key { get; set; }
            string Value { get; set; }
        }
        internal class TestConfiguration : ITestConfiguration
        {
            public string Key { get; set; }
            public string Value { get; set; }

        }

        internal class AppSettingsConfigurations
        {
            public IEnumerable<ITestConfiguration> Configurations { get; set; }
        }

        internal class TestConfigurations
        {
            public TestConfigurations(IEnumerable<ITestConfiguration> manualSettings, IOptionsSnapshot<AppSettingsConfigurations> appSettings)
            {
                Configurations = appSettings.Value.Configurations.Union(manualSettings, new TestConfigurationsComparer()).ToList();
            }

            public List<ITestConfiguration> Configurations { get; set; }
        }

        internal class TestConfigurationsComparer : IEqualityComparer<ITestConfiguration>
        {
            public bool Equals(ITestConfiguration x, ITestConfiguration y)
            {
                return x.Key.Equals(y.Key, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode([DisallowNull] ITestConfiguration obj)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + obj.Key.GetHashCode();
                    return hash;
                }
            }
        }

        [Fact]
        public void ReturnOptions1()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("ditest.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();

            services.Configure<ConfigurationTest1>(configuration.GetSection("test1"));
            var t1 = services.BuildServiceProvider().GetService<IOptions<ConfigurationTest1>>();
            Assert.Equal("Name1 Value", t1.Value.Name1);
            Assert.Equal("Name2 Value", t1.Value.Name2);

            var t2 = services.BuildServiceProvider().GetService<IOptionsSnapshot<ConfigurationTest1>>();
            Assert.Equal("Name1 Value", t2.Value.Name1);
            Assert.Equal("Name2 Value", t2.Value.Name2);
        }

        [Fact]
        public void ReturnOptions2()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("ditest.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();

            var t1 = new ConfigurationTest2();
            configuration.GetSection("test2").Bind(t1);
            services.AddSingleton(t1);
            var t2 = services.BuildServiceProvider().GetService<ConfigurationTest2>();
            Assert.Equal(2, t2.DITest.Count);

            services.Configure<ConfigurationTest2>(settings => configuration.GetSection("test2").Bind(settings));
            var t3 = services.BuildServiceProvider().GetService<IOptionsSnapshot<ConfigurationTest2>>();
            Assert.Equal(2, t3.Value.DITest.Count);
        }

        [Fact]
        public void ReturnOptions3()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("ditest.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();

            var t1 = configuration.GetSection("test1").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            services.AddSingleton(t1);
            var t2 = services.BuildServiceProvider().GetService<Dictionary<string, string>>();
            Assert.NotNull(t2);
            Assert.Equal(2, t2.Count);
        }

        [Fact]
        public void ReturnOptions4()
        {
            var dic1 = new Dictionary<string, string>()
            {
                {"test:Name1", "Value1" },
                {"test:Name2", "Value2" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dic1)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();

            services.Configure<ConfigurationTest2>(settings => settings.DITest = configuration.GetSection("test").GetChildren().ToDictionary(x => x.Key, x => x.Value));
            var t2 = services.BuildServiceProvider().GetService<IOptionsSnapshot<ConfigurationTest2>>();
            Assert.Equal(2, t2.Value.DITest.Count);
        }
        [Fact]
        public void Settings_Should_Update_Dictionary_Item()
        {
            var dic1 = new Dictionary<string, string>()
            {
                {"test:Name1", "Value1" },
                {"test:Name2", "Value2" }
            };

            var memSrc = new MemoryConfigurationSource { InitialData = dic1 };

            var configuration = new ConfigurationBuilder()
                .Add(memSrc)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();

            services.Configure<ConfigurationTest2>(settings => settings.DITest = configuration.GetSection("test").GetChildren().ToDictionary(x => x.Key, x => x.Value));
            var t2 = services.BuildServiceProvider().GetService<IOptionsSnapshot<ConfigurationTest2>>();
            Assert.Equal(2, t2.Value.DITest.Count);

            configuration["test:Name1"] = "ChangedValue1";
            var t3 = services.BuildServiceProvider().GetService<IOptionsSnapshot<ConfigurationTest2>>();
            Assert.Equal("ChangedValue1", t3.Value.DITest["Name1"]);

        }


        [Fact]
        public void Settings_Should_Append_Multiple_Dictionary_Item_Sources()
        {

            var dic1 = new Dictionary<string, string>()
            {
                {"test:Name1", "Value1" },
                {"test:Name2", "Value2" }
            };

            var memSrc = new MemoryConfigurationSource { InitialData = dic1 };

            var configuration = new ConfigurationBuilder()
                .Add(memSrc)
                .Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.AddTransient<TestConfigurations>();

            var c1 = new TestConfiguration() { Key = "Name3", Value = "Value3" };
            services.AddSingleton<ITestConfiguration>(c1);
            var c2 = new TestConfiguration() { Key = "Name4", Value = "Value4" };
            services.AddSingleton<ITestConfiguration>(c2);
            var c3 = new TestConfiguration() { Key = "Name1", Value = "Should_Not_Get_Included" };
            services.AddSingleton<ITestConfiguration>(c2);

            services.Configure<AppSettingsConfigurations>(c =>
                c.Configurations =
                    configuration.GetSection("test")
                        .GetChildren()
                        .Select(x => new TestConfiguration() { Key = x.Key, Value = x.Value })
                        .ToList());

            var t2 = services.BuildServiceProvider().GetService<TestConfigurations>();
            Assert.Equal(4, t2.Configurations.Count());
            Assert.Equal("Value1", t2.Configurations.First(a => a.Key == "Name1").Value);

            configuration["test:Name1"] = "Changed1";

            var t3 = services.BuildServiceProvider().GetService<TestConfigurations>();
            Assert.Equal("Changed1", t3.Configurations.First(a => a.Key == "Name1").Value);

        }

    }
}
