using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class TransactionManager
	{
		public class Transaction
		{
			private Guid m_guid;

			private CurrencyItem m_item;

			private string m_productId;

			public string Id
			{
				get
				{
					return m_guid.ToString();
				}
			}

			public CurrencyItem Item
			{
				get
				{
					if (m_item == null)
					{
						m_item = GameController.Instance.Store.FindProduct(m_productId);
					}
					return m_item;
				}
			}

			public Transaction(CurrencyItem item)
			{
				m_item = item;
				m_productId = m_item.ProductId;
				m_guid = Guid.NewGuid();
			}

			public Transaction(TransactionHistory.TransactionData data)
			{
				m_guid = new Guid(data.Id);
				m_productId = data.ProductId;
			}

			public TransactionHistory.TransactionData ToData()
			{
				TransactionHistory.TransactionData transactionData = new TransactionHistory.TransactionData();
				transactionData.Id = Id;
				transactionData.ProductId = m_productId;
				return transactionData;
			}

			public override bool Equals(object obj)
			{
				Transaction transaction = obj as Transaction;
				if (transaction != null)
				{
					return transaction.m_guid == m_guid;
				}
				return false;
			}

			public bool Equals(Transaction other)
			{
				if (other != null)
				{
					return other.m_guid == m_guid;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return m_guid.GetHashCode();
			}

			public override string ToString()
			{
				return m_item.ProductId + "__" + m_guid.ToString();
			}

			public static bool operator ==(Transaction first, Transaction second)
			{
				if (object.ReferenceEquals(first, second))
				{
					return true;
				}
				if (object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null))
				{
					return false;
				}
				return first.m_guid == second.m_guid;
			}

			public static bool operator !=(Transaction first, Transaction second)
			{
				if (object.ReferenceEquals(first, second))
				{
					return false;
				}
				if (object.ReferenceEquals(first, null) || object.ReferenceEquals(second, null))
				{
					return true;
				}
				return first.m_guid != second.m_guid;
			}
		}

		private List<Transaction> m_pendingTransactions = new List<Transaction>();

		private List<Transaction> m_completedTransactions = new List<Transaction>();

		public Transaction PendingTransaction
		{
			get
			{
				if (HasPendingTransaction)
				{
					return m_pendingTransactions[0];
				}
				return null;
			}
		}

		public bool HasPendingTransaction
		{
			get
			{
				return m_pendingTransactions != null && m_pendingTransactions.Count > 0;
			}
		}

		public bool HasAnyCompletedTransaction
		{
			get
			{
				return m_completedTransactions.Count > 0;
			}
		}

		public void Initialize()
		{
			TransactionHistory transactionHistory = Storage.Instance.LoadTransactionHistory();
			if (transactionHistory == null)
			{
				return;
			}
			foreach (TransactionHistory.TransactionData pendingTransaction in transactionHistory.PendingTransactions)
			{
				Transaction item = new Transaction(pendingTransaction);
				m_pendingTransactions.Add(item);
			}
			foreach (TransactionHistory.TransactionData completedTransaction in transactionHistory.CompletedTransactions)
			{
				Transaction item2 = new Transaction(completedTransaction);
				m_completedTransactions.Add(item2);
			}
		}

		public string CreatePendingTransaction(CurrencyItem item)
		{
			Transaction transaction = new Transaction(item);
			m_pendingTransactions.Add(transaction);
			UpdateTransactionHistory();
			return transaction.Id;
		}

		public void CompleteTransaction(Transaction pendingTransaction)
		{
			Transaction transaction = m_pendingTransactions.Find((Transaction x) => x == pendingTransaction);
			if (transaction != null)
			{
				m_pendingTransactions.Remove(transaction);
				m_completedTransactions.Add(transaction);
				UpdateTransactionHistory();
			}
			else
			{
				Debug.LogError("Unknown transaction tried to be completed! " + transaction.ToString());
			}
		}

		public Transaction CancelPendingTransaction()
		{
			if (!HasPendingTransaction)
			{
				Debug.LogWarning("No pending transaction to cancel!");
				return null;
			}
			Transaction transaction = m_pendingTransactions[0];
			m_pendingTransactions.Remove(transaction);
			UpdateTransactionHistory();
			return transaction;
		}

		private void UpdateTransactionHistory()
		{
			TransactionHistory transactionHistory = new TransactionHistory();
			List<TransactionHistory.TransactionData> list = new List<TransactionHistory.TransactionData>();
			foreach (Transaction pendingTransaction in m_pendingTransactions)
			{
				list.Add(pendingTransaction.ToData());
			}
			List<TransactionHistory.TransactionData> list2 = new List<TransactionHistory.TransactionData>();
			foreach (Transaction completedTransaction in m_completedTransactions)
			{
				list2.Add(completedTransaction.ToData());
			}
			transactionHistory.PendingTransactions = list;
			transactionHistory.CompletedTransactions = list2;
			if (CloudStorageST.ServerAvailable)
			{
				LocalStorage.Instance.UpdateTransactionHistory(transactionHistory);
			}
			Storage.Instance.UpdateTransactionHistory(transactionHistory);
		}
	}
}
