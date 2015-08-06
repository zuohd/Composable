namespace Composable.CQRS.EventSourcing.SQLServer
{
    internal class EventTableSchemaManager : TableSchemaManager
    {
        private static EventTable EventTable { get; } = new EventTable();
        override public string Name { get; } = "Events";

        override public string CreateTableSql => $@"
CREATE TABLE [dbo].[{Name}](
    {EventTable.Columns.Id} [bigint] IDENTITY(10,10) NOT NULL,
	{EventTable.Columns.AggregateId} [uniqueidentifier] NOT NULL,
	{EventTable.Columns.AggregateVersion} [int] NOT NULL,
	{EventTable.Columns.TimeStamp} [datetime] NOT NULL,
	{EventTable.Columns.SqlTimeStamp} [timestamp] NOT NULL,
	{EventTable.Columns.EventType} [varchar](300) NOT NULL,
    {EventTable.Columns.EventTypeId} int NULL,
	{EventTable.Columns.EventId} [uniqueidentifier] NOT NULL,
	{EventTable.Columns.Event} [nvarchar](max) NOT NULL
CONSTRAINT [IX_Uniq_{EventTable.Columns.EventId}] UNIQUE
(
	{EventTable.Columns.EventId}
),
CONSTRAINT [IX_Uniq_{EventTable.Columns.Id}] UNIQUE
(
	{EventTable.Columns.Id}
),
CONSTRAINT [PK_{Name}] PRIMARY KEY CLUSTERED 
(
	{EventTable.Columns.AggregateId} ASC,
	{EventTable.Columns.AggregateVersion} ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
) ON [PRIMARY]
CREATE UNIQUE NONCLUSTERED INDEX [{EventTable.Columns.SqlTimeStamp}] ON [dbo].[{Name}]
(
	[{EventTable.Columns.SqlTimeStamp}] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
";
    }
}