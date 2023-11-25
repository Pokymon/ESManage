import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function CreateItemSupplierTransaction() {
  const [id, setId] = useState("");
  const [itemId, setItemId] = useState("");
  const [supplierId, setSupplierId] = useState("");
  const [transactionType, setTransactionType] = useState("");
  const [transactionDate, setTransactionDate] = useState(""); // Example date
  const [quantity, setQuantity] = useState(0);
  const [notes, setNotes] = useState("");
  const [error, setError] = useState("");
  const [createdBy, setCreatedBy] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [items, setItems] = useState([]);
  const [suppliers, setSuppliers] = useState([]);
  const [itemSuppliers, setItemSuppliers] = useState([]);
  const [itemSupplierTransactions, setItemSupplierTransactions] = useState([]);
  const [suggestedSuppliers, setSuggestedSuppliers] = useState([]);

  const navigate = useNavigate();

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const fetchResource = async (url, setter) => {
    try {
      const response = await axios.get(url, {
        headers: {
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
      });
      setter(response.data);
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  };

  useEffect(() => {
    fetchResource("https://localhost:7240/api/item", setItems);
    fetchResource("https://localhost:7240/api/supplier", setSuppliers);
    fetchResource("https://localhost:7240/api/itemsupplier", setItemSuppliers);
  }, []);

  useEffect(() => {
    const fetchTransactionsAndEnrich = async () => {
      const transactionsResponse = await fetch(
        "https://localhost:7240/api/itemsupplier_transaction"
      );
      const transactionsData = await transactionsResponse.json();

      // Wait until items and suppliers data is fetched before enriching transactions
      if (items.length && suppliers.length && itemSuppliers.length) {
        const enrichedData = transactionsData.map((tx) => {
          // Find the corresponding itemSupplier
          const itemSupplier = itemSuppliers.find(
            (is) => is.id.toString() === tx.itemSupplierId.toString()
          );
          if (!itemSupplier) {
            return {
              ...tx,
              itemName: "Unknown Item",
              supplierName: "Unknown Supplier",
            };
          }
          // Find the actual item and supplier using itemId and supplierId from itemSupplier
          const item = items.find(
            (itm) => itm.id.toString() === itemSupplier.itemId.toString()
          );
          const supplier = suppliers.find(
            (sup) => sup.id.toString() === itemSupplier.supplierId.toString()
          );

          return {
            ...tx,
            itemName: item ? item.itemName : "Unknown Item", // Make sure 'name' is the correct property name for item name
            supplierName: supplier ? supplier.supplierName : "Unknown Supplier", // Make sure 'name' is the correct property name for supplier name
          };
        });

        setItemSupplierTransactions(enrichedData);
      }
    };

    fetchTransactionsAndEnrich();
  }, [items, suppliers, itemSuppliers]);

  const updateSuggestedSuppliers = (selectedItemId) => {
    // Filter the itemSuppliers to get suppliers for the selected item
    const associatedSuppliers = itemSuppliers
      .filter((is) => is.itemId.toString() === selectedItemId)
      .map((is) => suppliers.find((s) => s.id === is.supplierId));

    // Update the state to only show suppliers associated with the selected item
    setSuggestedSuppliers(associatedSuppliers);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      let response = await axios.post(
        "https://localhost:7240/api/itemsupplier_transaction",
        {
          id, // Assuming 'id' is part of your data model; remove if not needed.
          itemId,
          supplierId,
          transactionType,
          transactionDate,
          quantity,
          notes,
          createdBy, // Assuming 'createdBy' is part of your data model; remove if not needed.
        },
        {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
      console.log(response.data);
      // Reset states after successful submission
      setId("");
      setItemId("");
      setSupplierId("");
      setTransactionType("");
      setTransactionDate("");
      setQuantity(0);
      setNotes("");
      setCreatedBy(""); // Reset this as well if 'createdBy' is part of your data model
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
      <h2 className="page-title">Create a new Item Supplier Transaction</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="row">
              <div className="mb-3 col-6">
                <label className="form-label">Item</label>
                <select
                  className="form-select"
                  value={itemId}
                  onChange={(e) => {
                    setItemId(e.target.value);
                    updateSuggestedSuppliers(e.target.value);
                  }}
                >
                  <option value="">Select an item</option>
                  {items.map((item) => (
                    <option key={item.id} value={item.id}>
                      {item.itemName}
                    </option>
                  ))}
                </select>
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Supplier</label>
                <select
                  className="form-select"
                  value={supplierId}
                  onChange={(e) => setSupplierId(e.target.value)}
                  disabled={!itemId} // This will disable the dropdown if itemId is not set
                >
                  <option value="">Select a supplier</option>
                  {suggestedSuppliers.map((supplier) => (
                    <option key={supplier.id} value={supplier.id}>
                      {supplier.supplierName}
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

export default CreateItemSupplierTransaction;
