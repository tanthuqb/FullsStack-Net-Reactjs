import { useState, useEffect, useCallback } from "react";

export default function TaskTable({ onEdit, onDelete }) {
    const [tasks, setTasks] = useState([]);
    const [page, setPage] = useState(1);
    const [statusFilter, setStatusFilter] = useState("");
    const [sortOrder, setSortOrder] = useState("");
    const pageSize = 5;

    const fetchTasks = useCallback(async () => {
        const params = new URLSearchParams({
            page,
            pageSize,
            status: statusFilter,
            sort: sortOrder
        });

        const res = await fetch(`http://localhost:5000/api/TaskEntity/filter?${params}`, {
            credentials: "include"
        });
        if (res.ok) {
            const data = await res.json();
            setTasks(data);
        }
    }, [page, pageSize, statusFilter, sortOrder]);

    useEffect(() => {
        fetchTasks();
    }, [fetchTasks]);

    return (
        <div>
            <div className="d-flex gap-3 align-items-center mb-3">
                <select className="form-select w-auto" value={statusFilter} onChange={(e) => { setPage(1); setStatusFilter(e.target.value); }}>
                    <option value="">All Status</option>
                    <option value="NotStarted">Not Started</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select>
                <select className="form-select w-auto" value={sortOrder} onChange={(e) => { setPage(1); setSortOrder(e.target.value); }}>
                    <option value="">Sort by Due Date</option>
                    <option value="asc">Oldest First</option>
                    <option value="desc">Newest First</option>
                </select>
            </div>
            <table className="table-striped table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Status</th>
                        <th>Due Date</th>
                        <th>Description</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {tasks.map(task => (
                        <tr key={task.id}>
                            <td>{task.title}</td>
                            <td>{task.status}</td>
                            <td>{task.dueDate ? new Date(task.dueDate).toLocaleDateString("en-GB") : ""}</td>
                            <td>{task.description}</td>
                            <td>
                                <button className="btn btn-warning btn-sm me-2" onClick={() => onEdit(task)}>Edit</button>
                                <button className="btn btn-outline-danger btn-sm" onClick={() => onDelete(task.id)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                    {tasks.length === 0 && <tr><td colSpan="5" className="text-center">No tasks found.</td></tr>}
                </tbody>
            </table>
            <div className="d-flex justify-content-between mt-3">
                <button className="btn btn-secondary" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Previous</button>
                <span>Page {page}</span>
                <button className="btn btn-secondary" onClick={() => setPage(p => p + 1)} disabled={tasks.length < pageSize}>Next</button>
            </div>
        </div>
    );
}