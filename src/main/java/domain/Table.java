package domain;

import java.util.List;

public class Table {
    private String name;
    private List<Attribute> attributes;

    public Table(String name, List<Attribute> attributes) {
        this.name = name;
        this.attributes = attributes;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public List<Attribute> getAttributes() {
        return attributes;
    }

    public void setAttributes(List<Attribute> attributes) {
        this.attributes = attributes;
    }

    @Override
    public String toString() {
        return "Table{" +
                "name='" + name + '\'' +
                ", attributes=" + attributes +
                '}';
    }
}
