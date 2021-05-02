
<template>
  <div class="about">
    <router-link :to="{ name: 'Invoices' }">Back</router-link>

    <h2>Invoice Details</h2>

    <span>Invoice #{{ $route.params.id }}</span>

    <h3>Line Items</h3>

    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Quantity</th>
        <th>Cost</th>
        <th>Billable</th>
      </thead>
      <tbody>
        <tr v-for="item in state.lineItems" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.description }}</td>
          <td>{{ item.quantity }}</td>
          <td>{{ item.cost }}</td>
          <td>
            <input
              type="checkbox"
              :value="item.id"
              @change="onBillableChange(item.id)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <div class="flex-row-div">
      <div class="value-number-div">
        Total Value: <b>{{ totalValue }}</b>
      </div>
      <div class="value-number-div">
        Total Billable Value: <b>{{ totalBillableValue }}</b>
      </div>
    </div>

    <form @submit.prevent>
      <h4>Create Line Item</h4>
      <input
        type="text"
        name="description"
        placeholder="Description"
        v-model="state.description"
      />
      <input
        type="number"
        name="quantity"
        placeholder="Quantity"
        v-model="state.quantity"
      />
      <input
        type="number"
        name="cost"
        placeholder="Cost"
        v-model="state.cost"
      />
      <button @click="createLineItem">Create Invoice</button>
    </form>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, reactive } from "vue";

interface LineItem {
  id: number;
  invoiceId: number;
  description: string;
  quantity: number;
  cost: number;
}

export default defineComponent({
  name: "Invoice",
  props: {
    id: {
      type: [String, Number],
      required: true,
    },
  },
  setup(props) {
    const state = reactive({
      lineItems: <LineItem[]>[],
      description: "",
      quantity: "0",
      cost: "0",
      billableItemsId: <Number[]>[],
    });

    const totalValue = computed(() => {
      let sumValue: number = 0;
      const items: Array<LineItem> = state.lineItems;
      items.forEach((item) => {
        sumValue += item.quantity * item.cost;
      });
      return sumValue;
    });

    function fetchLineItems() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      }).then((response) => {
        response.json().then((lineItems) => {
          state.lineItems = lineItems;
          console.log(state.lineItems);
        });
      });
    }

    function createLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
        }),
      }).then(fetchLineItems);
    }

    function onBillableChange(id: number) {
      const index = state.billableItemsId.indexOf(id);
      if (index === -1) {
        state.billableItemsId.push(id);
      } else {
        state.billableItemsId.splice(index, 1);
      }
    }

    const totalBillableValue = computed(() => {
      let sumValue: number = 0;
      state.billableItemsId.forEach((id) => {
        state.lineItems.forEach((item) => {
          if (id === item.id) {
            sumValue += item.quantity * item.cost;
          }
        });
      });
      return sumValue;
    });

    onMounted(fetchLineItems);

    return {
      state,
      createLineItem,
      totalValue,
      onBillableChange,
      totalBillableValue,
    };
  },
});
</script>

<style scoped>
.flex-row-div {
  display: flex;
  flex-direction: row;
}

.value-number-div {
  margin: 0 10px;
}
</style>
