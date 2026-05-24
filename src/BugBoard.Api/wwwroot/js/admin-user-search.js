document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("adminUserFilterForm");
    const searchInput = document.getElementById("adminUserSearch");
    const roleFilter = document.getElementById("adminRoleFilter");
    const tableContainer = document.getElementById("adminUserTableContainer");

    if (!form || !searchInput || !roleFilter || !tableContainer) {
        return;
    }
    let timeoutId;

    const loadUsers = () => {
        const formData = new FormData(form);
        const queryString = new URLSearchParams(formData).toString();

        fetch(`/Admin/Search?${queryString}`, {
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to load users.");
                }
                return response.text();
            })
            .then(html => {
                tableContainer.innerHTML = html;
            })
            .catch(error => {
                console.error(error);
            });
    };
    searchInput.addEventListener("input", () => {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(loadUsers, 300);
    });
    roleFilter.addEventListener("change", loadUsers);
});