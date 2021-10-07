package domain;

import java.util.ArrayList;
import java.util.List;

public class Database {
    private String name;
    private List<Table> tableList;

    public Database(String name, List<Table> tableList) {
        this.name = name;
        this.tableList = tableList;
    }

    public Database() {

    }

    public Database(String name) {
        this.name = name;
        this.tableList = new ArrayList<>();
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public List<Table> getTableList() {
        return tableList;
    }

    public void setTableList(List<Table> tableList) {
        this.tableList = tableList;
    }

    @Override
    public String toString() {
        return "Database{" +
                "name='" + name + '\'' +
                ", tableList=" + tableList +
                '}';
    }
}
