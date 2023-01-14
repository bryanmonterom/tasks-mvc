async function handleApiErrors(response) {
    let message = '';

    if (response.status === 400) {
        message = await response.text();
    }
    if (response.status === 404) {
        message = resourceNotFound
    }
    else {

        message = unexpectedError;
    }

    showErrorMessage(message);
}

function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error ...',
        text: message
    })
}

function confirmAction({ callBackAccept, callBackCancel, title }) {

    Swal.fire({

        title: title || 'Are you sure?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        focusConfirm:true
    }).then((results) => {
        if (results.isConfirmed) {
            callBackAccept();
        }
        else if (callBackCancel) {
            callBackCancel();
        }
    })
}