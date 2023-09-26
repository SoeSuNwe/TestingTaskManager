using TaskManager.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moq; 
using TaskManager.Repositories;
using ModelTask = TaskManager.Models.Task;
namespace TestProject
{   
        public class TaskControllerTests
        {
            [Fact]
            public async Task Index_ReturnsViewWithAscendingSortedTasks()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                var controller = new MyTaskController(taskRepositoryMock.Object);

                var tasks = new List<ModelTask>
            {
                new ModelTask { TaskId = 1, DueDate = DateTime.Now.AddHours(1) },
                new ModelTask { TaskId = 2, DueDate = DateTime.Now.AddHours(2) },
                new  ModelTask { TaskId = 3, DueDate = DateTime.Now.AddHours(3) }
            };

                taskRepositoryMock.Setup(repo => repo.GetAllTasks())
                                  .ReturnsAsync(tasks);

                // Act
                var result = await controller.Index("ascending");

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<ModelTask>>(viewResult.ViewData.Model);
                var sortedTasks = model.ToList();

                Assert.Equal(1, sortedTasks[0].TaskId);
                Assert.Equal(2, sortedTasks[1].TaskId);
                Assert.Equal(3, sortedTasks[2].TaskId);
            }

            [Fact]
            public async Task Index_ReturnsViewWithDescendingSortedTasks()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                var controller = new MyTaskController(taskRepositoryMock.Object);

                var tasks = new List<TaskManager.Models.Task>
            {
                new ModelTask { TaskId = 1, DueDate = DateTime.Now.AddHours(1) },
                new ModelTask { TaskId = 2, DueDate = DateTime.Now.AddHours(2) },
                new ModelTask { TaskId = 3, DueDate = DateTime.Now.AddHours(3) }
            };

                taskRepositoryMock.Setup(repo => repo.GetAllTasks())
                                  .ReturnsAsync(tasks);

                // Act
                var result = await controller.Index("descending");

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<ModelTask>>(viewResult.ViewData.Model);
                var sortedTasks = model.ToList();

                Assert.Equal(3, sortedTasks[0].TaskId);
                Assert.Equal(2, sortedTasks[1].TaskId);
                Assert.Equal(1, sortedTasks[2].TaskId);
            }

            [Fact]
            public async Task Index_ReturnsNotFound_WhenSortOrderIsInvalid()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                var controller = new MyTaskController(taskRepositoryMock.Object);

                // Act
                var result = await controller.Index("invalidSortOrder");

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }

            [Fact]
            public async Task Details_ReturnsNotFound_WhenIdIsNull()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                var controller = new MyTaskController(taskRepositoryMock.Object);

                // Act
                var result = await controller.Details(null);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }

            [Fact]
            public async Task Details_ReturnsNotFound_WhenTaskIsNull()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((ModelTask)null);
                var controller = new MyTaskController(taskRepositoryMock.Object);

                // Act
                var result = await controller.Details(1); // Assuming the ID is 1

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
                Assert.Equal(404, notFoundResult.StatusCode);
            }

            [Fact]
            public async Task Details_ReturnsViewResult_WhenTaskExists()
            {
                // Arrange
                var taskRepositoryMock = new Mock<ITaskRepository>();
                taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync(new ModelTask { TaskId = 1, Title = "Test Task" });
                var controller = new MyTaskController(taskRepositoryMock.Object);

                // Act
                var result = await controller.Details(1); // Assuming the ID is 1

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.NotNull(viewResult.Model);
                Assert.IsType<ModelTask>(viewResult.Model);
            }
        
    }
}