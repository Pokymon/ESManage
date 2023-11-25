import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams, useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function EditItemSupplierTransaction() {
  const { id: urlId } = useParams();
  const navigate = useNavigate();

  const [id, setId] = useState(urlId);
  const [itemSupplierId, setItemSupplierId] = useState("");
  const [transactionType, setTransactionType] = useState("");
  const [transactionDate, setTransactionDate] = useState("");
  const [quantity, setQuantity] = useState(0);
  const [notes, setNotes] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [itemSuppliers, setItemSuppliers] = useState([]);

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const headersConfig = {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        };

        const itemSupplierResponse = await axios.get(
          "https://localhost:7240/api/itemsupplier",
          headersConfig
        );
        const transactionResponse = await axios.get(
          `https://localhost:7240/api/itemsupplier_transaction/${urlId}`,
          headersConfig
        );
        setItemSuppliers(itemSupplierResponse.data);
        const transactionData = transactionResponse.data;
        setId(transactionData.id);
        setItemSupplierId(transactionData.itemSupplierId);
        setTransactionType(transactionData.transactionType);
        setTransactionDate(transactionData.transactionDate);
        setQuantity(transactionData.quantity);
        setNotes(transactionData.notes);
      } catch (error) {
        console.error(error);
        setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [urlId]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      await axios.put(
        `https://localhost:7240/api/itemsupplier_transaction/${id}`,
        {
          id,
          itemSupplierId,
          transactionType,
          transactionDate,
          quantity,
          notes,
        },
        {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
      navigate("/item-supplier-transaction");
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Edit Item Supplier Transaction</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="row">
              <div className="mb-3 col-6">
                <label className="form-label">ID</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Enter the ID"
                  value={id}
                  onChange={(e) => setId(e.target.value)}
                  disabled
                />
              </div>
              {/* Dropdown for Item Supplier ID */}
              <div className="mb-3 col-6">
                <label className="form-label">Item Supplier ID</label>
                <select
                  className="form-select"
                  value={itemSupplierId}
                  onChange={(e) => setItemSupplierId(e.target.value)}
                  disabled
                >
                  <option value="">Select an Item Supplier</option>
                  {itemSuppliers.map((supplier) => (
                    <option key={supplier.id} value={supplier.id}>
                      {supplier.id}
                    </option>
                  ))}
                </select>
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Transaction Type</label>
                <select
                  className="form-select"
                  value={transactionType}
                  onChange={(e) => setTransactionType(e.target.value)}
                >
                  <option value="">Select a transaction type</option>
                  <option value="pembelian">Pembelian</option>
                  <option value="penerimaan">Penerimaan</option>
                  <option value="pengembalian">Pengembalian</option>
                  <option value="pengiriman">Pengiriman</option>
                </select>
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Transaction Date</label>
                <input
                  type="date"
                  className="form-control"
                  value={transactionDate}
                  onChange={(e) => setTransactionDate(e.target.value)}
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Quantity</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the quantity"
                  value={quantity}
                  onChange={(e) => setQuantity(e.target.value)}
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Notes</label>
                <textarea
                  className="form-control"
                  placeholder="Enter notes"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                ></textarea>
              </div>
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/item-supplier-transaction" className="btn btn-primary">
                Cancel
              </Link>
            </div>
          </form>
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}
        </div>
      </div>
    </AppLayout>
  );
}

export default EditItemSupplierTransaction;
