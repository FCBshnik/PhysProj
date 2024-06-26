﻿using Autofac;
using NodaTime;
using NodaTime.Extensions;
using Phys.HistoryDb;
using Phys.Mongo.HistoryDb;
using Shouldly;
using Testcontainers.MongoDb;
using Phys.Lib.Autofac;
using Phys.Lib.Tests.Db;
using MongoDB.Driver;

namespace Phys.Lib.Tests.Integration.Db
{
    public class MongoTests : DbTests
    {
        private readonly MongoDbContainer mongo = TestContainerFactory.CreateMongo();

        public MongoTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void HistoryDbTests()
        {
            using var container = BuildContainer();
            using var lifetimeScope = container.BeginLifetimeScope();
            var factory = lifetimeScope.Resolve<IHistoryDbFactory>();
            var db = factory.Create<HistoryTestDbo>("tests");
            var item1 = new HistoryTestDbo() { Id = db.GetNewId(), Data = "item1" };
            var item2 = new HistoryTestDbo() { Id = db.GetNewId(), Data = "item2" };
            db.Save(item1);
            db.Save(item2);
            item1.Id.ShouldNotBeNull();
            item2.Id.ShouldNotBeNull();

            var end = DateTime.UtcNow.ToInstant();
            var start = end.Minus(Duration.FromMinutes(1));
            var list = db.List(new HistoryDbQuery(new Interval(start, end), 0, 10));
            list.Count.ShouldBe(2);
            list[0].Data.ShouldBe("item2");
            list[1].Data.ShouldBe("item1");

            end = DateTime.UtcNow.ToInstant().Minus(Duration.FromMinutes(1));
            start = end.Minus(Duration.FromMinutes(1));
            list = db.List(new HistoryDbQuery(new Interval(start, end), 0, 10));
            list.Count.ShouldBe(0);

            end = DateTime.UtcNow.ToInstant().Plus(Duration.FromMinutes(2));
            start = end.Minus(Duration.FromMinutes(1));
            list = db.List(new HistoryDbQuery(new Interval(start, end), 0, 10));
            list.Count.ShouldBe(0);
        }

        protected override async Task Init()
        {
            await base.Init();
            await mongo.StartAsync();
        }

        protected override async Task Release()
        {
            await base.Release();
            await mongo.DisposeAsync();
        }

        protected override void Register(ContainerBuilder builder)
        {
            base.Register(builder);

            var mongoUrl = new MongoUrl(mongo.GetConnectionString());
            builder.RegisterModule(new MongoDbModule(mongoUrl, "test", loggerFactory));
            builder.Register(c => new MongoHistoryDbFactory(mongoUrl, "physlib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();
        }

        public class HistoryTestDbo : IHistoryDbo
        {
            public required string Id { get; set; }

            public string? Data { get; set; }
        }
    }
}
