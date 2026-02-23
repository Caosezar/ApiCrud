USE ApiCrudDB;
GO

-- =============================================
-- CRIAÇÃO DA TABELA TASKS
-- Sistema de Gerenciamento de Tarefas
-- =============================================

-- Verificar se a tabela já existe e removê-la
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[Tasks];
    PRINT 'Tabela Tasks removida.';
END
GO

-- Criar tabela Tasks
CREATE TABLE [dbo].[Tasks]
(
    -- Identificação
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,

    -- Informações principais
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,

    -- Classificação
    [Priority] NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'ToDo',
    [CategoryId] INT NULL,

    -- Datas
    [DueDate] DATETIME NULL,
    [CompletedAt] DATETIME NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NULL,

    -- Tags (armazenadas como JSON ou string separada por vírgula)
    [Tags] NVARCHAR(500) NULL,

    -- Campos de controle
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [Position] INT NULL,
    -- Para ordenação customizada (Kanban)

    -- Constraints
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tasks_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tasks_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id]) ON DELETE SET NULL,
    CONSTRAINT [CHK_Tasks_Priority] CHECK ([Priority] IN ('Low', 'Medium', 'High', 'Urgent')),
    CONSTRAINT [CHK_Tasks_Status] CHECK ([Status] IN ('ToDo', 'InProgress', 'Done', 'Cancelled'))
);
GO

PRINT 'Tabela Tasks criada com sucesso.';
GO

-- =============================================
-- ÍNDICES PARA PERFORMANCE
-- =============================================

-- Índice composto para consultas por usuário e status (mais comum)
CREATE NONCLUSTERED INDEX [IX_Tasks_UserId_Status] 
ON [dbo].[Tasks] ([UserId], [Status])
INCLUDE ([Title], [Priority], [DueDate], [CreatedAt]);
GO

-- Índice para consultas por data de vencimento
CREATE NONCLUSTERED INDEX [IX_Tasks_DueDate] 
ON [dbo].[Tasks] ([DueDate])
WHERE [DueDate] IS NOT NULL AND [Status] <> 'Done' AND [Status] <> 'Cancelled';
GO

-- Índice para consultas por categoria
CREATE NONCLUSTERED INDEX [IX_Tasks_CategoryId] 
ON [dbo].[Tasks] ([CategoryId])
WHERE [CategoryId] IS NOT NULL;
GO

-- Índice para ordenação Kanban (por posição)
CREATE NONCLUSTERED INDEX [IX_Tasks_Position] 
ON [dbo].[Tasks] ([UserId], [Status], [Position]);
GO

-- Índice para tarefas não arquivadas (consultas mais frequentes)
CREATE NONCLUSTERED INDEX [IX_Tasks_Active] 
ON [dbo].[Tasks] ([IsArchived], [Status])
WHERE [IsArchived] = 0;
GO

PRINT 'Índices criados com sucesso.';
GO

-- =============================================
-- TRIGGER - ATUALIZAR UpdatedAt AUTOMATICAMENTE
-- =============================================

CREATE TRIGGER [dbo].[TR_Tasks_UpdateTimestamp]
ON [dbo].[Tasks]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Tasks]
    SET [UpdatedAt] = GETDATE()
    FROM [dbo].[Tasks] t
        INNER JOIN inserted i ON t.Id = i.Id;
END
GO

PRINT 'Trigger TR_Tasks_UpdateTimestamp criada.';
GO

-- =============================================
-- TRIGGER - ATUALIZAR CompletedAt QUANDO STATUS = 'Done'
-- =============================================

CREATE TRIGGER [dbo].[TR_Tasks_SetCompletedAt]
ON [dbo].[Tasks]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Marcar data de conclusão quando status mudar para Done
    UPDATE [dbo].[Tasks]
    SET [CompletedAt] = GETDATE()
    FROM [dbo].[Tasks] t
        INNER JOIN inserted i ON t.Id = i.Id
        INNER JOIN deleted d ON t.Id = d.Id
    WHERE i.[Status] = 'Done'
        AND d.[Status] <> 'Done'
        AND t.[CompletedAt] IS NULL;

    -- Remover data de conclusão se status mudar de Done para outro
    UPDATE [dbo].[Tasks]
    SET [CompletedAt] = NULL
    FROM [dbo].[Tasks] t
        INNER JOIN inserted i ON t.Id = i.Id
        INNER JOIN deleted d ON t.Id = d.Id
    WHERE i.[Status] <> 'Done'
        AND d.[Status] = 'Done';
END
GO

PRINT 'Trigger TR_Tasks_SetCompletedAt criada.';
GO

-- =============================================
-- TABELA DE CATEGORIAS DE TAREFAS (SE NÃO EXISTIR)
-- =============================================

IF NOT EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[TaskCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TaskCategories]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Icon] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT [PK_TaskCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_TaskCategories_Name] UNIQUE ([Name])
    );

    PRINT 'Tabela TaskCategories criada.';

    -- Atualizar a FK se a tabela foi criada agora
    ALTER TABLE [dbo].[Tasks]
    DROP CONSTRAINT [FK_Tasks_Categories];

    ALTER TABLE [dbo].[Tasks]
    ADD CONSTRAINT [FK_Tasks_TaskCategories] 
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[TaskCategories]([Id]) ON DELETE SET NULL;

    PRINT 'Foreign Key atualizada para TaskCategories.';
END
GO

-- =============================================
-- INSERIR CATEGORIAS PADRÃO
-- =============================================

IF NOT EXISTS (SELECT *
FROM [dbo].[TaskCategories])
BEGIN
    INSERT INTO [dbo].[TaskCategories]
        ([Name], [Description], [Icon])
    VALUES
        ('Trabalho', 'Tarefas relacionadas ao trabalho', 'briefcase'),
        ('Pessoal', 'Tarefas pessoais e do dia a dia', 'user'),
        ('Estudos', 'Estudos e cursos', 'book'),
        ('Projetos', 'Projetos específicos', 'folder'),
        ('Reuniões', 'Reuniões e compromissos', 'calendar'),
        ('Urgente', 'Tarefas urgentes e importantes', 'alert-circle');

    PRINT 'Categorias padrão inseridas.';
END
GO

-- =============================================
-- INSERIR DADOS DE EXEMPLO
-- =============================================

-- Verificar se existe pelo menos um usuário
IF EXISTS (SELECT TOP 1
    1
FROM [dbo].[Users])
BEGIN
    DECLARE @UserId INT = (SELECT TOP 1
        Id
    FROM [dbo].[Users]
    ORDER BY Id);
    DECLARE @WorkCategoryId INT = (SELECT Id
    FROM [dbo].[TaskCategories]
    WHERE Name = 'Trabalho');
    DECLARE @PersonalCategoryId INT = (SELECT Id
    FROM [dbo].[TaskCategories]
    WHERE Name = 'Pessoal');
    DECLARE @StudyCategoryId INT = (SELECT Id
    FROM [dbo].[TaskCategories]
    WHERE Name = 'Estudos');

    INSERT INTO [dbo].[Tasks]
        ([UserId], [Title], [Description], [Priority], [Status], [CategoryId], [DueDate], [Tags], [Position])
    VALUES
        (@UserId, 'Implementar CRUD de Tasks', 'Criar backend e frontend para gerenciamento de tarefas', 'High', 'InProgress', @WorkCategoryId, DATEADD(DAY, 2, GETDATE()), 'backend,crud,api', 1),
        (@UserId, 'Estudar React Hooks', 'Revisar useState, useEffect e useContext', 'Medium', 'ToDo', @StudyCategoryId, DATEADD(DAY, 5, GETDATE()), 'react,javascript,frontend', 2),
        (@UserId, 'Fazer compras do mês', 'Lista: arroz, feijão, frutas, verduras', 'Medium', 'ToDo', @PersonalCategoryId, DATEADD(DAY, 1, GETDATE()), 'casa,compras', 3),
        (@UserId, 'Revisar PR do colega', 'Revisar pull request #123 do projeto X', 'Urgent', 'ToDo', @WorkCategoryId, GETDATE(), 'code-review,trabalho', 4),
        (@UserId, 'Documentar API', 'Criar documentação completa da API com Swagger', 'High', 'ToDo', @WorkCategoryId, DATEADD(DAY, 7, GETDATE()), 'documentação,api', 5),
        (@UserId, 'Marcar consulta médica', 'Agendar check-up anual', 'Low', 'Done', @PersonalCategoryId, DATEADD(DAY, -5, GETDATE()), 'saúde', NULL);

    -- Atualizar CompletedAt da tarefa concluída
    UPDATE [dbo].[Tasks] 
    SET [CompletedAt] = DATEADD(DAY, -5, GETDATE())
    WHERE [Title] = 'Marcar consulta médica';

    PRINT 'Dados de exemplo inseridos.';
END
ELSE
BEGIN
    PRINT 'Nenhum usuário encontrado. Insira usuários primeiro.';
END
GO

-- =============================================
-- VIEWS ÚTEIS
-- =============================================

-- View: Tarefas ativas por usuário
CREATE OR ALTER VIEW [dbo].[vw_ActiveTasks]
AS
    SELECT
        t.Id,
        t.UserId,
        u.FirstName + ' ' + u.LastName AS UserName,
        t.Title,
        t.Description,
        t.Priority,
        t.Status,
        c.Name AS CategoryName,
        c.Icon AS CategoryIcon,
        t.DueDate,
        t.Tags,
        t.CreatedAt,
        CASE 
        WHEN t.DueDate IS NULL THEN NULL
        WHEN t.DueDate < GETDATE() THEN 'Overdue'
        WHEN t.DueDate < DATEADD(DAY, 1, GETDATE()) THEN 'DueToday'
        WHEN t.DueDate < DATEADD(DAY, 3, GETDATE()) THEN 'DueSoon'
        ELSE 'OnTime'
    END AS DueStatus,
        DATEDIFF(DAY, GETDATE(), t.DueDate) AS DaysUntilDue
    FROM [dbo].[Tasks] t
        INNER JOIN [dbo].[Users] u ON t.UserId = u.Id
        LEFT JOIN [dbo].[TaskCategories] c ON t.CategoryId = c.Id
    WHERE t.IsArchived = 0
        AND t.Status NOT IN ('Done', 'Cancelled');
GO

PRINT 'View vw_ActiveTasks criada.';
GO

-- View: Estatísticas de tarefas por usuário
CREATE OR ALTER VIEW [dbo].[vw_TaskStats]
AS
    SELECT
        UserId,
        COUNT(*) AS TotalTasks,
        SUM(CASE WHEN Status = 'ToDo' THEN 1 ELSE 0 END) AS ToDoCount,
        SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgressCount,
        SUM(CASE WHEN Status = 'Done' THEN 1 ELSE 0 END) AS DoneCount,
        SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledCount,
        SUM(CASE WHEN DueDate < GETDATE() AND Status NOT IN ('Done', 'Cancelled') THEN 1 ELSE 0 END) AS OverdueCount,
        SUM(CASE WHEN Priority = 'Urgent' AND Status NOT IN ('Done', 'Cancelled') THEN 1 ELSE 0 END) AS UrgentCount
    FROM [dbo].[Tasks]
    WHERE IsArchived = 0
    GROUP BY UserId;
GO

PRINT 'View vw_TaskStats criada.';
GO

-- =============================================
-- STORED PROCEDURES ÚTEIS
-- =============================================

-- Procedure: Obter tarefas com filtros
CREATE OR ALTER PROCEDURE [dbo].[sp_GetTasks]
    @UserId INT,
    @Status NVARCHAR(20) = NULL,
    @Priority NVARCHAR(20) = NULL,
    @CategoryId INT = NULL,
    @SearchTerm NVARCHAR(200) = NULL,
    @IncludeArchived BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Id,
        t.UserId,
        t.Title,
        t.Description,
        t.Priority,
        t.Status,
        t.CategoryId,
        c.Name AS CategoryName,
        c.Icon AS CategoryIcon,
        t.DueDate,
        t.CompletedAt,
        t.CreatedAt,
        t.UpdatedAt,
        t.Tags,
        t.Position,
        CASE 
            WHEN t.DueDate < GETDATE() AND t.Status NOT IN ('Done', 'Cancelled') THEN 1
            ELSE 0
        END AS IsOverdue
    FROM [dbo].[Tasks] t
        LEFT JOIN [dbo].[TaskCategories] c ON t.CategoryId = c.Id
    WHERE t.UserId = @UserId
        AND (@Status IS NULL OR t.Status = @Status)
        AND (@Priority IS NULL OR t.Priority = @Priority)
        AND (@CategoryId IS NULL OR t.CategoryId = @CategoryId)
        AND (@IncludeArchived = 1 OR t.IsArchived = 0)
        AND (@SearchTerm IS NULL OR
        t.Title LIKE '%' + @SearchTerm + '%' OR
        t.Description LIKE '%' + @SearchTerm + '%' OR
        t.Tags LIKE '%' + @SearchTerm + '%')
    ORDER BY 
        CASE WHEN t.Position IS NOT NULL THEN t.Position ELSE 9999 END,
        t.CreatedAt DESC;
END
GO

PRINT 'Procedure sp_GetTasks criada.';
GO

-- Procedure: Reordenar tarefas (Kanban)
CREATE OR ALTER PROCEDURE [dbo].[sp_ReorderTasks]
    @TaskId INT,
    @NewPosition INT,
    @NewStatus NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldPosition INT;
    DECLARE @OldStatus NVARCHAR(20);
    DECLARE @UserId INT;

    -- Obter dados atuais
    SELECT
        @OldPosition = Position,
        @OldStatus = Status,
        @UserId = UserId
    FROM [dbo].[Tasks]
    WHERE Id = @TaskId;

    -- Se mudar de status, usar o novo status
    IF @NewStatus IS NOT NULL
    BEGIN
        SET @OldStatus = @NewStatus;
    END

    -- Ajustar posições das outras tarefas
    IF @NewPosition < @OldPosition
    BEGIN
        -- Movendo para cima
        UPDATE [dbo].[Tasks]
        SET Position = Position + 1
        WHERE UserId = @UserId
            AND Status = @OldStatus
            AND Position >= @NewPosition
            AND Position < @OldPosition
            AND Id <> @TaskId;
    END
    ELSE IF @NewPosition > @OldPosition
    BEGIN
        -- Movendo para baixo
        UPDATE [dbo].[Tasks]
        SET Position = Position - 1
        WHERE UserId = @UserId
            AND Status = @OldStatus
            AND Position <= @NewPosition
            AND Position > @OldPosition
            AND Id <> @TaskId;
    END

    -- Atualizar a tarefa movida
    UPDATE [dbo].[Tasks]
    SET 
        Position = @NewPosition,
        Status = ISNULL(@NewStatus, Status)
    WHERE Id = @TaskId;
END
GO

PRINT 'Procedure sp_ReorderTasks criada.';
GO

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================

DECLARE @TaskCount INT, @CategoryCount INT;

SELECT @TaskCount = COUNT(*)
FROM [dbo].[Tasks];
SELECT @CategoryCount = COUNT(*)
FROM [dbo].[TaskCategories];

PRINT '=============================================';
PRINT '✅ TABELA TASKS CRIADA COM SUCESSO';
PRINT '=============================================';
PRINT '';
PRINT '📊 RESUMO:';
PRINT '   • Tabela Tasks criada';
PRINT '   • Tabela TaskCategories criada';
PRINT '   • ' + CAST(@CategoryCount AS VARCHAR) + ' categorias inseridas';
PRINT '   • ' + CAST(@TaskCount AS VARCHAR) + ' tarefas de exemplo inseridas';
PRINT '   • 5 índices criados para performance';
PRINT '   • 2 triggers criados (UpdatedAt e CompletedAt)';
PRINT '   • 2 views criadas (ActiveTasks e TaskStats)';
PRINT '   • 2 stored procedures criadas';
PRINT '';
PRINT '🔍 QUERIES ÚTEIS:';
PRINT '   • SELECT * FROM Tasks;';
PRINT '   • SELECT * FROM vw_ActiveTasks;';
PRINT '   • SELECT * FROM vw_TaskStats;';
PRINT '   • EXEC sp_GetTasks @UserId = 1;';
PRINT '';
PRINT '🚀 Tabela pronta para uso!';
PRINT '=============================================';
GO