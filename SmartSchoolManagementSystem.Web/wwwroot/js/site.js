// Common utility functions
const utils = {
    formatDate: (date) => {
        return new Date(date).toLocaleDateString();
    },
    
    formatCurrency: (amount) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    showToast: (message, type = 'success') => {
        const toast = document.createElement('div');
        toast.className = `toast show position-fixed top-0 end-0 m-3 bg-${type} text-white`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <div class="toast-body d-flex align-items-center">
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'} me-2"></i>
                ${message}
            </div>
        `;
        document.body.appendChild(toast);
        setTimeout(() => {
            toast.remove();
        }, 3000);
    },

    confirmDelete: (message = 'Are you sure you want to delete this item?') => {
        return new Promise((resolve) => {
            const modal = document.createElement('div');
            modal.className = 'modal fade';
            modal.innerHTML = `
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Confirm Delete</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <p>${message}</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteBtn">Delete</button>
                        </div>
                    </div>
                </div>
            `;
            document.body.appendChild(modal);
            const modalInstance = new bootstrap.Modal(modal);
            modalInstance.show();

            modal.querySelector('#confirmDeleteBtn').addEventListener('click', () => {
                modalInstance.hide();
                resolve(true);
            });

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
                resolve(false);
            });
        });
    }
};

// Form validation
document.addEventListener('DOMContentLoaded', () => {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        if (form.classList.contains('needs-validation')) {
            form.addEventListener('submit', event => {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            });
        }
    });
});

// Data table initialization
const initDataTable = (tableId, options = {}) => {
    const defaultOptions = {
        pageLength: 10,
        responsive: true,
        language: {
            search: 'Search:',
            lengthMenu: 'Show _MENU_ entries',
            info: 'Showing _START_ to _END_ of _TOTAL_ entries',
            paginate: {
                first: '<i class="fas fa-angle-double-left"></i>',
                last: '<i class="fas fa-angle-double-right"></i>',
                next: '<i class="fas fa-angle-right"></i>',
                previous: '<i class="fas fa-angle-left"></i>'
            }
        }
    };

    return new DataTable(`#${tableId}`, { ...defaultOptions, ...options });
};

// File input preview
const initFilePreview = (inputId, previewId) => {
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewId);

    if (input && preview) {
        input.addEventListener('change', () => {
            const file = input.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    preview.src = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        });
    }
};

// Dynamic form fields
const initDynamicFields = (containerId, addButtonId, template) => {
    const container = document.getElementById(containerId);
    const addButton = document.getElementById(addButtonId);

    if (container && addButton) {
        addButton.addEventListener('click', () => {
            const index = container.children.length;
            const newField = template.replace(/\{index\}/g, index);
            container.insertAdjacentHTML('beforeend', newField);

            // Initialize any components in the new field
            const newRow = container.lastElementChild;
            newRow.querySelector('.remove-field')?.addEventListener('click', () => {
                newRow.remove();
            });
        });
    }
};

// Search functionality
const initSearch = (inputId, items, searchKeys, displayCallback) => {
    const searchInput = document.getElementById(inputId);
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const filteredItems = items.filter(item => {
                return searchKeys.some(key => {
                    const value = item[key]?.toString().toLowerCase() ?? '';
                    return value.includes(searchTerm);
                });
            });
            displayCallback(filteredItems);
        });
    }
};

// Export functionality
const exportToExcel = (tableId, fileName) => {
    const table = document.getElementById(tableId);
    if (table) {
        const wb = XLSX.utils.table_to_book(table);
        XLSX.writeFile(wb, `${fileName}.xlsx`);
    }
};

// Print functionality
const printElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <html>
                <head>
                    <title>Print</title>
                    <link href="/css/site.css" rel="stylesheet" />
                </head>
                <body>
                    ${element.outerHTML}
                    <script>window.onload = () => window.print();</script>
                </body>
            </html>
        `);
        printWindow.document.close();
    }
};
