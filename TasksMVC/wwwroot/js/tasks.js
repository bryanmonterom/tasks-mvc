function addNewTask() {
    tasksListViewModel.tasks.push(new taskElementViewModel({ id: 0, title: '' }))
    $("[name='task-title']").last().focus();
}

async function handleFocustOutTaskTitle(task) {
    const title = task.title();
    if (!title) {
        tasksListViewModel.tasks.pop();
        return;
    }

    const data = JSON.stringify(title);

    const response = await fetch(urlTasks, {
        method: 'POST',
        body: data,
        headers: { 'Content-Type': 'application/json' }
    })

    if (response.ok) {
        const json = await response.json();
        task.id(json.id);
    }
    else {
        handleApiErrors(response)

    }
}

async function getTasks() {

    tasksListViewModel.loading(true);
    const response = await fetch(`${urlTasks}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (!response.ok) {
        handleApiErrors(response)
    }


    const json = await response.json();
    tasksListViewModel.tasks([]);

    json.forEach((item) => {
        tasksListViewModel.tasks.push(new taskElementViewModel(item));
    });


    tasksListViewModel.loading(false);
}

async function sendIds(ids) {

    var data = JSON.stringify(ids);
    await fetch(`${urlTasks}/Sort`, {
        method: 'POST',
        body: data,
        headers: { 'Content-Type': 'application/json' }
    });
};

async function getTasksId() {
    const ids = $("[name=task-title]").map(function () {
        return $(this).attr("data-id")
    }).get();

    return ids;
}
async function updatePositions() {
    const ids = await getTasksId();
    await sendIds(ids);


    const sortedArray = tasksListViewModel.tasks.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString())
    })

    tasksListViewModel.tasks([]);
    tasksListViewModel.tasks(sortedArray)


}

async function handleTaskClick(task) {
    if (task.isNew()) {
        return;
    }

    const response = await fetch(`${urlTasks}/${task.id()}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }

    });

    if (!response.ok) {
        handleApiErrors(response);
        return;
    }

    const json = await response.json();

    taskEditViewModel.id = json.id;
    taskEditViewModel.title(json.title);
    taskEditViewModel.description(json.description)
    taskEditViewModel.steps([])




    json.subTasks.forEach((step) => {
        taskEditViewModel.steps.push(new stepViewModel({ ...step, editingMode: false }))
    })

    modalEditTaskBootstrap.show();

}
$(function () {
    $("#tasks-resortable").sortable({
        axis: 'y',
        stop: async function () {
            await updatePositions();
        }
    })
})

async function handleChangeEditTask() {

    const obj = {
        id: taskEditViewModel.id,
        title: taskEditViewModel.title(),
        description: taskEditViewModel.description()
    }

    if (!obj.title) {
        return;
    }


    await editTask(obj);

    const index = tasksListViewModel.tasks().findIndex(a => a.id() === obj.id);

    const task = tasksListViewModel.tasks()[index];
    task.title(obj.title)

}

async function editTask(task) {

    const data = JSON.stringify(task);

    const response = await fetch(`${urlTasks}/${task.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        },
    });

    if (!response.ok) {
        handleApiErrors(response);
        throw "Error";
    }
}

function tryToDeleteTask(task) {

    modalEditTaskBootstrap.hide();

    confirmAction({
        callBackAccept: () => { deleteTask(task); },
        callBackCancel: () => { modalEditTaskBootstrap.show() },
        title: `Do you want to delete the task ${task.title()}?`
    });
}

async function deleteTask(task) {

    const idTask = task.id;
    const response = await fetch(`${urlTasks}/${idTask}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (response.ok) {
        const index = getIndexTaskEditing()
        tasksListViewModel.tasks.splice(index, 1);
    }


}

function getIndexTaskEditing() {
    return tasksListViewModel.tasks().findIndex(a => a.id() == taskEditViewModel.id);
}

function getTaskEditing() {
    const index = getIndexTaskEditing();
    return tasksListViewModel.tasks()[index];
}