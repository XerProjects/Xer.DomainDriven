using System;

namespace Xer.DomainDriven.Tests.Entities
{
    public class TestEntity : Entity
    {
        public TestEntity(Guid entityId) : base(entityId)
        {
        }

        public TestEntity(Guid entityId, DateTime created, DateTime updated) : base(entityId, created, updated)
        {
        }
    }

    public class TestEntitySecond : Entity
    {
        public TestEntitySecond(Guid entityId) : base(entityId)
        {
        }

        public TestEntitySecond(Guid entityId, DateTime created, DateTime updated) : base(entityId, created, updated)
        {
        }
    }
}