BEGIN TRANSACTION;
GO

ALTER TABLE [DDRs] ADD [Item] nvarchar(200) NULL;
GO

ALTER TABLE [DDRs] ADD [MobileNumber] nvarchar(20) NULL;
GO

ALTER TABLE [DDRs] ADD [PartyId] int NULL;
GO

ALTER TABLE [DDRs] ADD [Rent] decimal(18,2) NULL;
GO

ALTER TABLE [CashBookEntries] ADD [Balance] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [CashBookEntries] ADD [PaymentId] int NULL;
GO

ALTER TABLE [CashBookEntries] ADD [ReceiptId] int NULL;
GO

ALTER TABLE [CashBookEntries] ADD [ReferenceNumber] nvarchar(100) NULL;
GO

                ;WITH Ledger AS
                (
                    SELECT [Id],
                        SUM([Debit] - [Credit]) OVER
                        (ORDER BY [Date], [Id] ROWS UNBOUNDED PRECEDING) AS [CalculatedBalance]
                    FROM [CashBookEntries]
                    WHERE [IsDeleted] = 0
                )
                UPDATE [CashBookEntries]
                SET [Balance] = Ledger.[CalculatedBalance]
                FROM [CashBookEntries]
                INNER JOIN Ledger ON Ledger.[Id] = [CashBookEntries].[Id];
GO

CREATE INDEX [IX_DDRs_PartyId] ON [DDRs] ([PartyId]);
GO

CREATE INDEX [IX_CashBookEntries_Date] ON [CashBookEntries] ([Date]);
GO

CREATE UNIQUE INDEX [IX_CashBookEntries_PaymentId] ON [CashBookEntries] ([PaymentId]) WHERE [PaymentId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_CashBookEntries_ReceiptId] ON [CashBookEntries] ([ReceiptId]) WHERE [ReceiptId] IS NOT NULL;
GO

CREATE INDEX [IX_CashBookEntries_ReferenceNumber] ON [CashBookEntries] ([ReferenceNumber]);
GO

ALTER TABLE [CashBookEntries] ADD CONSTRAINT [FK_CashBookEntries_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [CashBookEntries] ADD CONSTRAINT [FK_CashBookEntries_Receipts_ReceiptId] FOREIGN KEY ([ReceiptId]) REFERENCES [Receipts] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [DDRs] ADD CONSTRAINT [FK_DDRs_Parties_PartyId] FOREIGN KEY ([PartyId]) REFERENCES [Parties] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260818054617_ExtendDdrAndCashBookLedger', N'8.0.0');
GO

COMMIT;
GO

