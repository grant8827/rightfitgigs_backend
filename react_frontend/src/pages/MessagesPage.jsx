import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { 
  getUserConversations, 
  getConversationMessages, 
  sendMessage as sendMessageAPI,
  markConversationAsRead 
} from '../services/apiService';
import './MessagesPage.css';

const MessagesPage = ({ initialRecipient = null, onBack = null }) => {
  const { user } = useAuth();
  const [conversations, setConversations] = useState([]);
  const [selectedConversation, setSelectedConversation] = useState(null);
  const [messages, setMessages] = useState([]);
  const [newMessage, setNewMessage] = useState('');

  useEffect(() => {
    loadConversations();
    if (initialRecipient && user) {
      // Create or select conversation with the initial recipient
      const recipient = initialRecipient;
      // Check if conversation already exists
      const existing = conversations.find(
        c => c.recipientId === recipient.id
      );
      
      if (existing) {
        setSelectedConversation(existing);
        loadMessages(existing.conversationId);
      } else {
        // Create new conversation placeholder - will get real ID when first message is sent
        const participants = [user.id, recipient.id].sort();
        const tempConversationId = `${participants[0]}_${participants[1]}`;
        
        const newConv = {
          id: tempConversationId,
          conversationId: tempConversationId,
          recipientId: recipient.id,
          recipientName: recipient.name,
          recipientType: recipient.userType || 'Worker',
          jobId: recipient.jobId || null,
          jobTitle: recipient.jobTitle || null,
          lastMessage: null,
          lastMessageTime: new Date(),
          unreadCount: 0
        };
        setConversations([newConv, ...conversations]);
        setSelectedConversation(newConv);
        setMessages([]);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialRecipient, user]);

  const loadConversations = async () => {
    try {
      if (!user?.id) return;
      const data = await getUserConversations(user.id);
      const formattedConversations = data.map(conv => ({
        id: conv.conversationId,
        conversationId: conv.conversationId,
        recipientId: conv.otherParticipantId,
        recipientName: conv.otherParticipantName,
        recipientType: conv.otherParticipantType,
        jobId: conv.jobId,
        jobTitle: conv.jobTitle,
        lastMessage: conv.lastMessageContent,
        lastMessageTime: new Date(conv.lastMessageDate),
        unreadCount: conv.unreadCount,
        hasUnreadMessages: conv.hasUnreadMessages
      }));
      setConversations(formattedConversations);
    } catch (error) {
      console.error('Failed to load conversations:', error);
    }
  };

  const loadMessages = async (conversationId) => {
    try {
      const data = await getConversationMessages(conversationId);
      const formattedMessages = data.map(msg => ({
        id: msg.id,
        senderId: msg.senderId,
        senderName: msg.senderName,
        recipientId: msg.receiverId,
        content: msg.content,
        timestamp: new Date(msg.sentDate),
        read: msg.isRead
      }));
      setMessages(formattedMessages);
      
      // Mark conversation as read
      if (user?.id) {
        await markConversationAsRead(conversationId, user.id);
        // Refresh conversations to update unread count
        loadConversations();
      }
    } catch (error) {
      console.error('Failed to load messages:', error);
    }
  };

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!newMessage.trim() || !selectedConversation || !user) return;

    try {
      const messageData = {
        senderId: user.id,
        senderName: `${user.firstName} ${user.lastName}`,
        senderType: user.userType,
        receiverId: selectedConversation.recipientId,
        receiverName: selectedConversation.recipientName,
        receiverType: selectedConversation.recipientType,
        content: newMessage,
        conversationId: selectedConversation.conversationId,
        jobId: selectedConversation.jobId
      };
      
      const sentMessage = await sendMessageAPI(messageData);
      
      // Add message to local state (optimistic update)
      const formattedMessage = {
        id: sentMessage.id,
        senderId: sentMessage.senderId,
        senderName: sentMessage.senderName,
        recipientId: sentMessage.receiverId,
        content: sentMessage.content,
        timestamp: new Date(sentMessage.sentDate),
        read: sentMessage.isRead
      };
      
      setMessages([...messages, formattedMessage]);
      setNewMessage('');
      
      // Update conversation's last message
      setConversations(conversations.map(c => 
        c.conversationId === selectedConversation.conversationId
          ? { ...c, lastMessage: newMessage, lastMessageTime: new Date() }
          : c
      ));
    } catch (error) {
      console.error('Failed to send message:', error);
      alert('Failed to send message. Please try again.');
    }
  };

  const canInitiateConversation = user?.userType?.toLowerCase() === 'employer';

  return (
    <div className="messages-page">
      {onBack && (
        <button className="btn-back" onClick={onBack}>
          ← Back
        </button>
      )}
      
      <div className="messages-container">
        {/* Conversations List */}
        <div className="conversations-list">
          <div className="conversations-header">
            <h2>Messages</h2>
          </div>
          
          {!canInitiateConversation && conversations.length === 0 && (
            <div className="empty-state">
              <p>No messages yet</p>
              <small>You'll see messages here when employers contact you</small>
            </div>
          )}
          
          {conversations.length === 0 && canInitiateConversation && (
            <div className="empty-state">
              <p>No conversations yet</p>
              <small>Start a conversation by messaging a candidate</small>
            </div>
          )}
          
          {conversations.map((conv) => (
            <div
              key={conv.conversationId}
              className={`conversation-item ${selectedConversation?.conversationId === conv.conversationId ? 'active' : ''}`}
              onClick={() => {
                setSelectedConversation(conv);
                loadMessages(conv.conversationId);
              }}
            >
              <div className="conversation-avatar">
                {conv.recipientName.charAt(0)}
              </div>
              <div className="conversation-info">
                <div className="conversation-name">{conv.recipientName}</div>
                {conv.lastMessage && (
                  <div className="conversation-last-message">
                    {conv.lastMessage}
                  </div>
                )}
              </div>
              {conv.unreadCount > 0 && (
                <div className="unread-badge">{conv.unreadCount}</div>
              )}
            </div>
          ))}
        </div>

        {/* Messages Area */}
        <div className="messages-area">
          {selectedConversation ? (
            <>
              <div className="messages-header">
                <div>
                  <h3>{selectedConversation.recipientName}</h3>
                  {selectedConversation.jobTitle && (
                    <small style={{ color: '#2196F3', fontWeight: '500' }}>
                      Re: {selectedConversation.jobTitle}
                    </small>
                  )}
                </div>
              </div>
              
              <div className="messages-list">
                {messages.length === 0 ? (
                  <div className="empty-messages">
                    <p>No messages yet</p>
                    <small>Start the conversation!</small>
                  </div>
                ) : (
                  messages.map((msg) => (
                    <div
                      key={msg.id}
                      className={`message ${msg.senderId === user.id ? 'sent' : 'received'}`}
                    >
                      <div className="message-content">
                        <div className="message-sender">{msg.senderName}</div>
                        <div className="message-text">{msg.content}</div>
                        <div className="message-time">
                          {new Date(msg.timestamp).toLocaleString()}
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>
              
              <form className="message-input-form" onSubmit={handleSendMessage}>
                <input
                  type="text"
                  value={newMessage}
                  onChange={(e) => setNewMessage(e.target.value)}
                  placeholder="Type your message..."
                  className="message-input"
                />
                <button type="submit" className="btn-send">
                  Send
                </button>
              </form>
            </>
          ) : (
            <div className="no-conversation-selected">
              <p>Select a conversation to view messages</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default MessagesPage;
