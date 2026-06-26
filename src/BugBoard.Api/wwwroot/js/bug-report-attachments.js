(function () {
    function updateInputFiles(input, files) {
        var dataTransfer = new DataTransfer();

        files.forEach(function (file) {
            dataTransfer.items.add(file);
        });

        input.files = dataTransfer.files;
    }

    function renderSelectedFiles(input, listElement, files) {
        listElement.innerHTML = "";

        files.forEach(function (file, index) {
            var item = document.createElement("div");
            item.className = "bug-report-selected-attachment";

            var name = document.createElement("span");
            name.className = "bug-report-selected-attachment-name";
            name.textContent = file.name;

            var removeButton = document.createElement("button");
            removeButton.className = "bug-report-selected-attachment-remove";
            removeButton.type = "button";
            removeButton.setAttribute("aria-label", "Remove " + file.name);
            removeButton.textContent = "x";

            removeButton.addEventListener("click", function () {
                files.splice(index, 1);
                updateInputFiles(input, files);
                renderSelectedFiles(input, listElement, files);
            });

            item.appendChild(name);
            item.appendChild(removeButton);
            listElement.appendChild(item);
        });
    }

    document.querySelectorAll(".js-attachment-input").forEach(function (input) {
        var listSelector = input.getAttribute("data-selected-list");
        if (!listSelector) {
            return;
        }

        var listElement = document.querySelector(listSelector);
        if (!listElement) {
            return;
        }

        var selectedFiles = [];

        input.addEventListener("change", function () {
            selectedFiles = Array.from(input.files);
            renderSelectedFiles(input, listElement, selectedFiles);
        });
    });

    document.querySelectorAll("[data-attachments-toggle]").forEach(function (button) {
        var listSelector = button.getAttribute("data-attachments-toggle");
        var listElement = document.querySelector(listSelector);

        if (!listElement) {
            return;
        }

        button.addEventListener("click", function () {
            var isCollapsed = listElement.classList.toggle("is-collapsed");
            button.setAttribute("aria-expanded", (!isCollapsed).toString());
        });
    });
})();
