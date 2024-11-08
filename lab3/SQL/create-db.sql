CREATE DATABASE Student;
GO

USE Student;
GO

CREATE TABLE [dbo].[Student](
                                [StudentId] [int] IDENTITY(1,1) NOT NULL,
                                [RegistrationNumber] [varchar](7) NOT NULL,
                                [Name] [varchar](50) NOT NULL,
                                CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED
                                    (
                                     [StudentId] ASC
                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Grade](
                              [GradeId] [int] IDENTITY(1,1) NOT NULL,
                              [StudentId] [int] NOT NULL,
                              [Exam] [decimal](18, 2) NULL,
                              [Activity] [decimal](18, 2) NULL,
                              [Final] [decimal](18, 2) NULL,
                              CONSTRAINT [PK_Grades] PRIMARY KEY CLUSTERED
                                  (
                                   [GradeId] ASC
                                      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Grade]  WITH CHECK ADD  CONSTRAINT [FK_Grades_Student] FOREIGN KEY([StudentId])
    REFERENCES [dbo].[Student] ([StudentId])
GO

ALTER TABLE [dbo].[Grade] CHECK CONSTRAINT [FK_Grades_Student]
GO

