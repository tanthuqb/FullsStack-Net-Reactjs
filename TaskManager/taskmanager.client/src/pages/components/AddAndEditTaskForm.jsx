import { useState, useEffect } from "react";

export default function AddAndEditTaskForm({ onAdd, onUpdate, editingTask, clearEdit }) {
    const [form, setForm] = useState({
        title: "",
        description: "",
        status: "NotStarted",
        dueDate: ""
    });

    useEffect(() => {
        if (editingTask) {
            setForm({ ...editingTask, dueDate: editingTask.dueDate?.substring(0, 10) });
        } else {
            setForm({ title: "", description: "", status: "NotStarted", dueDate: "" });
        }
    }, [editingTask]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const url = editingTask
            ? `http://localhost:5000/api/TaskEntity/${editingTask.id}`
            : "http://localhost:5000/api/TaskEntity";
        const method = editingTask ? "PUT" : "POST";

        const res = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(form)
        });

        if (res.ok) {
            const data = editingTask ? null : await res.json();
            editingTask ? onUpdate() : onAdd(data);
            clearEdit();
        } else {
            alert("Error submitting task");
        }
    };

    return (
        <form onSubmit={handleSubmit} className="mb-4">
            <input type="text" name="title" className="form-control mb-2" placeholder="Title" value={form.title} onChange={handleChange} required />
            <textarea name="description" className="form-control mb-2" placeholder="Description" value={form.description} onChange={handleChange} />
            <select name="status" className="form-select mb-2" value={form.status} onChange={handleChange}>
                <option value="NotStarted">Not Started</option>
                <option value="InProgress">In Progress</option>
                <option value="Completed">Completed</option>
            </select>
            <input type="date" name="dueDate" className="form-control mb-2" value={form.dueDate} onChange={handleChange} />
            <button className="btn btn-primary w-100" type="submit">{editingTask ? "Update Task" : "Add Task"}</button>
        </form>
    );
} 