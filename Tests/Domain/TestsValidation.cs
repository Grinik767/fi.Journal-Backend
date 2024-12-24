using NUnit.Framework;
using Domain.Entities;
using Domain.Validators;

namespace Domain.Tests;

public class TestsValidation
{
    
    public class ValidatorTests
    {
        [Test]
        public void GroupValidator_ValidName_PassesValidation()
        {
            var group = new Group(Guid.NewGuid(), "ValidName", Guid.NewGuid());
            var validator = new GroupValidator();

            var result = validator.Validate(group);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void GroupValidator_InvalidName_FailsValidation()
        {
            var group = new Group(Guid.NewGuid(), "No", Guid.NewGuid());
            var validator = new GroupValidator();

            var result = validator.Validate(group);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void TableValidator_ValidTable_PassesValidation()
        {
            var table = new Table(
                Guid.NewGuid(),
                "ValidName",
                "https://docs.google.com/spreadsheets/d/12345",
                Guid.NewGuid(),
                1,
                "StudentColumn");
            var validator = new TableValidator();

            var result = validator.Validate(table);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void TableValidator_InvalidUrl_FailsValidation()
        {
            var table = new Table(
                Guid.NewGuid(),
                "ValidName",
                "http://invalid-url.com",
                Guid.NewGuid(),
                1,
                "StudentColumn");
            var validator = new TableValidator();

            var result = validator.Validate(table);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UserDiffValidator_ValidUpdateTime_PassesValidation()
        {
            var diff = new UserDiff(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new Dictionary<string, string>());
            var validator = new UserDiffValidator();

            var result = validator.Validate(diff);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void UserDiffValidator_NonUtcUpdateTime_FailsValidation()
        {
            var diff = new UserDiff(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new Dictionary<string, string>());
            diff.GetType().GetProperty("UpdateTime")?.SetValue(diff, DateTime.Now);

            var validator = new UserDiffValidator();
            var result = validator.Validate(diff);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UserValidator_ValidUser_PassesValidation()
        {
            var user = new User(Guid.NewGuid(), "ValidUser", "valid@example.com", "hash123");
            var validator = new UserValidator();

            var result = validator.Validate(user);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void UserValidator_InvalidEmail_FailsValidation()
        {
            var user = new User(Guid.NewGuid(), "ValidUser", "invalid-email", "hash123");
            var validator = new UserValidator();

            var result = validator.Validate(user);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UserValidator_EmptyPasswordHash_FailsValidation()
        {
            var user = new User(Guid.NewGuid(), "ValidUser", "valid@example.com", "");
            var validator = new UserValidator();

            var result = validator.Validate(user);

            Assert.That(result.IsValid, Is.False);
        }
    }
}