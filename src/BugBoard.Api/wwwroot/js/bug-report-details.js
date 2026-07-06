document.addEventListener('DOMContentLoaded', function () {
    const notifyStatus = document.getElementById('notify-status');
    const notifyComment = document.getElementById('notify-comment');

    function saveSubscription()
    {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const bugReportId = document.querySelector('input[name="bugReportId"]').value;

        const formData = new FormData();
        formData.append('__RequestVerificationToken', token);
        formData.append('bugReportId', bugReportId);
        formData.append('notifyOnStatusChange', notifyStatus.checked);
        formData.append('notifyOnComment', notifyComment.checked);

        fetch('/BugReports/SaveSubscription', { method: 'POST', body: formData })
            .then(response => {
                if (response.ok) {
                    const toastEl = document.getElementById('subscription-toast');
                    const toast = new bootstrap.Toast(toastEl);
                    toast.show();
                }
            });
    }

    if (notifyStatus) notifyStatus.addEventListener('change', saveSubscription);
    if (notifyComment) notifyComment.addEventListener('change', saveSubscription);
})