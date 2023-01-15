function handleClickAddStep() {

    const index = taskEditViewModel.steps().findIndex(a => a.isNew());

    if (index !== -1) {
        $("[name=txtStepDescription]:visible").focus();
    }

    taskEditViewModel.steps.push(new stepViewModel({ editingMode: true, isCompleted: false }))
    $("[name=txtStepDescription]:visible").focus();

}


async function handleSaveStep(step) {
    step.editingMode(false);
    const isNew = step.isNew();
    const taskId = taskEditViewModel.id;
    const data = getPetitionBody(step);
    const description = step.description();

    if (!description) {
        step.description(step.previousDescription);
        if (isNew) {
            taskEditViewModel.steps.pop();
            return;
        }
    }

    if (isNew) {
        await insertStep(step, data, taskId);
    }
    else {
        updateStep(data, step.id())
    }
}

async function insertStep(step, data, taskId) {
    console.log(data);
    const response = await fetch(`${urlSteps}/${taskId}`, {
        body: data,
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {

        const json = await response.json();
        console.log(json)
        step.id(json.id);

        const task = getTaskEditing();
        console.log(task)

        task.stepsTotal(task.stepsTotal() + 1)
        if (step.isCompleted()) {
            task.stepsCompleted(task.stepsCompleted() + 1)

        }
    }
    else {
        handleApiErrors(response);
    }
}

function getPetitionBody(step) {
    return JSON.stringify({
        description: step.description(),
        isCompleted: step.isCompleted()
    });
}

function handleCancelSaveStep(step) {

    if (step.isNew()) {
        taskEditViewModel.steps.pop()
        return
    }
    else {
        step.editingMode(false);
        step.description(step.previousDescription)
    }
}

function handleDescriptionStep(step) {

    step.editingMode(true);
    step.previousDescription = step.description();

    $("[name=txtStepDescription]:visible").focus();

}

async function updateStep(data, id) {
    const response = await fetch(`${urlSteps}/${id}`, {
        method: 'Put',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
        handleApiErrors(response);
    }


}


function handleCheckboxStep(step) {


    if (step.isNew()) {
        return true;
    }

    step.isCompleted(step.isCompleted());
    const data = getPetitionBody(step);
    console.log(data)
    updateStep(data, step.id())

    const task = getTaskEditing();
    let currentStepsCompleted = task.stepsCompleted();

    if (step.isCompleted()) {

        currentStepsCompleted++
    }
    else {
        currentStepsCompleted--
    }

    task.stepsCompleted(currentStepsCompleted);

    return true;

}

function handleClickDeleteStep(step) {

    modalEditTaskBootstrap.hide();
    confirmAction({
        callBackAccept : () => {

            deleteStep(step);
            modalEditTaskBootstrap.show();
        }, callBackCancel : () => {

            modalEditTaskBootstrap.show();
        }, title:"Are you sure that you want to delete this step?"
    });
}

async function deleteStep(step) {
    const response = await fetch(`${urlSteps}/${step.id()}`,{
        method: 'DELETE'
    });

    if (!response.ok) {
        handleApiErrors(response);
        return
    }

    taskEditViewModel.steps.remove((item) => item.id() == step.id())

    const task = getTaskEditing();
    task.stepsTotal(task.stepsTotal() - 1)

    if (step.isCompleted()) {
        task.stepsCompleted(task.stepsCompleted() - 1)
    }

}

function getIdSteps() {

    var ids = $("[name=chxStep]").map(function () {
        return $(this).attr('data-id');
    }).get()

    return ids;
}

async function updateStepPositions() {

    const ids = getIdSteps();
    await sendIdsStepsToBackedn(ids);

    const sortedArray = taskEditViewModel.steps.sorted(function (a, b) {

        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString())
    });

    taskEditViewModel.steps(sortedArray);


}

async function sendIdsStepsToBackedn(ids) {

    var data = JSON.stringify(ids);

    const response = await fetch(`${urlSteps}/Sort/${taskEditViewModel.id}`, {
        method: 'POST',
        body: data,
        headers: {
        'Content-Type':'application/json'
        }
    })
}

$(function () {

    $("#resortable-steps").sortable({
        axis: 'y',
        stop: async function () {
            await updateStepPositions();
        }
    })

})