﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PointOfInterestSkill.Dialogs.Shared.Resources;
using VirtualAssistant.Tests.TestHelpers;
using VirtualAssistant.Tests.Utterances;

namespace VirtualAssistant.Tests.SkillInvocationTests
{
    [TestClass]
    public class SkillInvocationTests : SkillTestBase
    {
        private Dictionary<string, SkillDefinition> skillDefinitions;

        [TestInitialize]
        public void InitSkills()
        {
            this.skillDefinitions = new Dictionary<string, SkillDefinition>();
            this.skillDefinitions.Add("calendarSkill", this.CreateSkillDefinition("calendarSkill", typeof(CalendarSkill.CalendarSkill)));
            this.skillDefinitions.Add("emailSkill", this.CreateSkillDefinition("emailSkill", typeof(EmailSkill.EmailSkill)));
            this.skillDefinitions.Add("toDoSkill", this.CreateSkillDefinition("todoSkill", typeof(ToDoSkill.ToDoSkill)));
            this.skillDefinitions.Add("pointOfInterestSkill", this.CreateSkillDefinition("pointOfInterestSkill", typeof(PointOfInterestSkill.PointOfInterestSkill)));

            foreach (SkillDefinition skill in this.skillDefinitions.Values)
            {
                // Add the SkillDialog to the available dialogs passing the initialized FakeSkill
                this.Dialogs.Add(new SkillDialog(skill, this.Services, this.ProactiveState, this.EndpointService, this.TelemetryClient, this.BackgroundTaskQueue));
            }
        }

        /// <summary>
        /// Test that we can invoke the Calendar Skill. If we are able to do this we will get an OAuth card.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task CalendarSkillInvocation()
        {
            this.SkillDialogOptions = new SkillDialogOptions();
            this.SkillDialogOptions.SkillDefinition = this.skillDefinitions["calendarSkill"];

            await this.GetTestFlow()
                .Send(CalendarUtterances.BookMeeting)
                .AssertReply((activity) => Assert.AreEqual(ActivityTypes.Trace, activity.Type))
                .AssertReply((activity) =>
                {
                    var messageActivity = activity.AsMessageActivity();
                    Assert.AreEqual(1, messageActivity.Attachments.Count);
                    Assert.AreEqual("application/vnd.microsoft.card.oauth", messageActivity.Attachments[0].ContentType);
                })
                .StartTestAsync();
        }

        /// <summary>
        /// Test that we can invoke the Email Skill. If we are able to do this we will get an OAuth card.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task EmailSkillInvocation()
        {
            this.SkillDialogOptions = new SkillDialogOptions();
            this.SkillDialogOptions.SkillDefinition = this.skillDefinitions["emailSkill"];

            await this.GetTestFlow()
                .Send(EmailUtterances.SendEmail)
                .AssertReply((activity) => Assert.AreEqual(ActivityTypes.Trace, activity.Type))
                .AssertReply((activity) =>
                {
                    var messageActivity = activity.AsMessageActivity();
                    Assert.AreEqual(1, messageActivity.Attachments.Count);
                    Assert.AreEqual("application/vnd.microsoft.card.oauth", messageActivity.Attachments[0].ContentType);
                })
                .StartTestAsync();
        }

        /// <summary>
        /// Test that we can invoke the ToDo Skill. If we are able to do this we will get an OAuth card.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task ToDoSkillInvocation()
        {
            this.SkillDialogOptions = new SkillDialogOptions();
            this.SkillDialogOptions.SkillDefinition = this.skillDefinitions["toDoSkill"];

            await this.GetTestFlow()
                .Send(ToDoUtterances.AddToDo)
                .AssertReply((activity) => Assert.AreEqual(ActivityTypes.Trace, activity.Type))
                .AssertReply((activity) =>
                {
                    var messageActivity = activity.AsMessageActivity();
                    Assert.AreEqual(1, messageActivity.Attachments.Count);
                    Assert.AreEqual("application/vnd.microsoft.card.oauth", messageActivity.Attachments[0].ContentType);
                })
                .StartTestAsync();
        }

        /// <summary>
        /// Test that we can invoke the PointOfInterest Skill, if we are able to do this we will get a trace activity saying that the
        /// Azure Maps key isn't available (through configuration) which is expected and proves we can invoke the skill.
        /// </summary>
        /// <returns>Task.</returns>
        [TestMethod]
        public async Task PointOfInterestSkillInvocation()
        {
            this.SkillDialogOptions = new SkillDialogOptions();
            this.SkillDialogOptions.SkillDefinition = this.skillDefinitions["pointOfInterestSkill"];

            await this.GetTestFlow()
            .Send(PointOfInterestUtterances.FindCoffeeShop)
            .AssertReply(this.ValidateAzureMapsKeyPrompt())
            .StartTestAsync();
        }

        private Action<IActivity> ValidateAzureMapsKeyPrompt()
        {
            return activity =>
            {
                var traceActivity = activity as Activity;
                Assert.IsNotNull(traceActivity);

                Assert.IsTrue(traceActivity.Text.Contains("DialogException: Could not get the required Azure Maps key. Please make sure your settings are correctly configured."));
            };
        }
    }
}
