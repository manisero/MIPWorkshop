#region

using System.Collections.Generic;
using FizzWare.NBuilder;
using MIP.DAL;
using MIP.DAL.Specifications;
using MIP.MarketBackend.BLL.DataManagement.Dashboard.Impl;
using MIP.MarketBackend.BLL.Entities.Dashboard;
using MIP.MarketBackend.DAL.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

#endregion

namespace MIP.MarketBackend.BLL.Tests.DataManagement.Dashboard
{
	[TestClass]
	public class SummaryBuilderTests : TestsBase
	{
		[TestMethod]
		public void BuildOperatorsTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var operatorsSummaryBuilder = new OperatorsSummaryBuilder(summaryList, toolId);
			ISummary summary = operatorsSummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildDevicesTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var devicesSummaryBuilder = new DevicesSummaryBuilder(summaryList, toolId);
			ISummary summary = devicesSummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildTouchpointsTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var touchpointsSummaryBuilder = new TouchpointsSummaryBuilder(summaryList, toolId);
			ISummary summary = touchpointsSummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildRespondentsTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var respondentsSummaryBuilder = new RespondentsSummaryBuilder(summaryList, toolId);
			ISummary summary = respondentsSummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildProfilesTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var profilesSummaryBuilder = new ProfilesSummaryBuilder(summaryList, toolId);
			ISummary summary = profilesSummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildActivitiesPerDayTest()
		{
			const int toolId = 15;
			var summaryList = new List<DashboardSummary>();
			var activitiesPerDaySummaryBuilder = new ActivitiesPerDaySummaryBuilder(summaryList, toolId);
			ISummary summary = activitiesPerDaySummaryBuilder.Build();
			Assert.IsNotNull(summary);
		}

		[TestMethod]
		public void BuildOperatorsTicketTest()
		{
			const int toolId = 15;

			IList<VisitorsSummary> summaries = Builder<VisitorsSummary>.CreateListOfSize(10).Build();

			AutoMoqer.GetMock<IRepository<VisitorsSummary>>()
				.Setup(x => x.Filter(It.IsAny<ISpecification<VisitorsSummary>>()))
				.Returns(summaries);

			var summaryList = new List<DashboardSummary>();

			var operatorsSummaryBuilder = new OperatorsTicketSummaryBuilder(
				summaryList, AutoMoqer.GetMock<IRepository<VisitorsSummary>>().Object, toolId);

			ISummary operatorsSummary = operatorsSummaryBuilder.Build();

			Assert.IsNotNull(operatorsSummary);

			VerifyAllMocks();
		}
	}
}
