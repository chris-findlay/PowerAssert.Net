﻿using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    // ReSharper disable EqualExpressionComparison

    [TestFixture]
    public class BrokenEqualityHintTests
    {
        [Test]
        public void DoesntTriggerWithDifferingObjectsForMethodEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new object();
            var y = new object();

            Expression<Func<bool>> exp = () => x.Equals(y);
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsFalse(hint.TryGetHint(p, exp.Body, out ignored));

            Assert.IsNull(ignored);
        }

        [Test]
        public void TriggersOnSameObjectForMethodEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new object();

            Expression<Func<bool>> exp = () => x.Equals(x);
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out ignored));
            Assert.IsNotNull(ignored);
        }


        [Test]
        public void TriggersOnSameObjectForOperatorEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new object();

            Expression<Func<bool>> exp = () => x == x;
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out ignored));

            Assert.IsNotNull(ignored);
        }

        [Test]
        public void DoesntTriggerWithDifferingObjectsForOperatorEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new object();
            var y = new object();

            Expression<Func<bool>> exp = () => x == y;
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsFalse(hint.TryGetHint(p, exp.Body, out ignored));

            Assert.IsNull(ignored);
        }


        [Test]
        public void PicksUpCorrectTypeForOverriddenMethodEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new OverridesEqualsBadly();

            Expression<Func<bool>> exp = () => x.Equals(x);
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out ignored));

            Assert.IsNotNull(ignored);
            Assert.IsTrue(ignored.Contains(x.GetType().Name));
        }

        [Test]
        public void PicksUpCorrectTypeForOverriddenOperatorEquals()
        {
            var hint = new BrokenEqualityHint();

            var x = new OverridesEqualsBadly();

            Expression<Func<bool>> exp = () => x == x;
            var p = new ExpressionParser(exp.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out ignored));

            Assert.IsNotNull(ignored);
            Assert.IsTrue(ignored.Contains(x.GetType().Name));
        }

        class OverridesEqualsBadly
        {
            public override int GetHashCode()
            {
                return 0;
            }

            public override bool Equals(object obj)
            {
                return false;
            }

            public static bool operator ==(OverridesEqualsBadly l, OverridesEqualsBadly r)
            {
                return false;
            }

            public static bool operator !=(OverridesEqualsBadly l, OverridesEqualsBadly r)
            {
                return true;
            }
        }
    }
}