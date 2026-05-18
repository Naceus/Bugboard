document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("bugReportFilterForm");
    const searchInput = document.getElementById("titleSearch");
    const tableContainer = document.getElementById("bugReportTableContainer");

    if (!form || !searchInput || !tableContainer) {
        return;
    }

    let debounceTimer;

    function updateTable() {
        const formData = new FormData(form);
        const queryString = new URLSearchParams(formData).toString();

        fetch(`/BugReports/Search?${queryString}`, {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Search request failed.");
                }

                return response.text();
            })
            .then(html => {
                tableContainer.innerHTML = html;
            })
            .catch(error => {
                console.error(error);
            });
    }

    searchInput.addEventListener("input", function () {
        clearTimeout(debounceTimer);

        debounceTimer = setTimeout(function () {
            updateTable();
        }, 400);
    });

    form.addEventListener("change", function () {
        updateTable();
    });

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        updateTable();
    });
});