let inputFile = document.getElementById('fileToTask');

function handleClickAddFile() {
    inputFile.click();
}

async function handleSelectionTaskFile(e) {

    const files = e.target.files;
    const filesArray = Array.from(files);

    const idTask = taskEditViewModel.id;
    const formData = new FormData();

    for (var i = 0; i < filesArray.length; i++) {
        formData.append("filesfromForm", filesArray[i]);
    }

    const response = await fetch(`${urlFiles}/${idTask}`, {
        body: formData,
        method: 'post'
    })

    if (!response.ok) {
        handleApiErrors(response);
        return;
    }

    const json = await response.json();

    inputFile.value = null;
}

function prepareFiles(attachedFiles) {
    attachedFiles.forEach(file => {
        let date = file.createdDate;
        if (file.createdDate.indexOf('Z') === -1) {
            date += 'Z';
        }
        const creationDate = new Date(date);
        file.published = creationDate.toLocaleDateString();
        console.log({...file})
        taskEditViewModel.files.push(new attachedFielViewModel({ ...file, editingMode: false }))

    })
} 

function handleClickAddFile() {
    inputFile.click();
}

async function handleSelectionTaskFile(e) {

    const files = e.target.files;
    const filesArray = Array.from(files);

    const idTask = taskEditViewModel.id;
    const formData = new FormData();

    for (var i = 0; i < filesArray.length; i++) {
        formData.append("filesfromForm", filesArray[i]);
    }

    const response = await fetch(`${urlFiles}/${idTask}`, {
        body: formData,
        method: 'post'
    })

    if (!response.ok) {
        handleApiErrors(response);
        return;
    }

    const json = await response.json();

    prepareFiles(json);

    inputFile.value = null;
}

function prepareFiles(attachedFiles) {

    attachedFiles.forEach(file => {

        let date = file.createdDate;
        if (file.createdDate.indexOf('Z') === -1) {
            date += 'Z';
        }

        const creationDate = new Date(date);
        file.published = creationDate.toLocaleDateString();

        taskEditViewModel.files.push(new attachedFielViewModel({ ...file, editingMode: false }))

    });
}
let previousFileTitle;
function handleClickFileTitle(file) {
    file.editingMode(true);
    previousFileTitle = file.title();
    $("[attachedFileTitle]:visible").focus();

}

async function handleFocusOutFileTitle(file) {
    file.editingMode(false);
    const id = file.id;


    if (!file.title()) {
        file.title(previousFileTitle)
    }
    console.log(file.title())
    console.log(previousFileTitle)


    if (file.title() === previousFileTitle) {

        return;
    }
    console.log('Im here')



    const data = JSON.stringify(file.title())

    const response = await fetch(`${urlFiles}/${id}`, {
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

function handleClickDeleteFile(file) {

    modalEditTaskBootstrap.hide();

    confirmAction({
        callBackAccept: () => {
            deleteFile(file);
            modalEditTaskBootstrap.show();

        }, callBackCancel: () => {
            modalEditTaskBootstrap.show();
        }, title: 'Are you sure you want to delete this file?'
    })
}

async function deleteFile(file) {

    const response = await fetch(`${urlFiles}/${file.id}`, {
        method:'DELETE'
    })

    if (!response.ok) {
        handleApiErrors(response);
    }

    taskEditViewModel.files.remove((item) => item.id === file.id);

}

function handleClickDownloadFile(file) {
    downloadFile(file.url, file.title());

}